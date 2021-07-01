using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeBunny.Core.Message
{
    public sealed class MessageProperties
    {
        public string MessageId { get; set; } 
        public string CorrelationId { get; set; } = Guid.NewGuid().ToString("N");
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(10);
        public DeliveryMode DeliveryMode { get; set; } = DeliveryMode.Persistent;
        public bool Persistent { get; set; } = true;
        public Dictionary<string, object> Headers { get; set; } = new();

        public TimeSpan DeliveryDelay
        {
            get => Headers.TryGetValue("x-delay", out var value) 
                    ? TimeSpan.Parse(Encoding.UTF8.GetString(value as byte[] ?? new byte[0])) 
                    : TimeSpan.Zero;
            set => Headers["x-delay"] = Encoding.UTF8.GetBytes(value.ToString());
        }

        public uint ContinuationMarker
        {
            get => Headers.TryGetValue(nameof(ContinuationMarker), out var marker) 
                ? uint.Parse(Encoding.UTF8.GetString(marker as byte[])) 
                : 0;
            set => Headers[nameof(ContinuationMarker)] = Encoding.UTF8.GetBytes(value.ToString());
        }

        public int RetryAttempt
        {
            get => Headers.TryGetValue(nameof(RetryAttempt), out var attempt)
                ? int.Parse(Encoding.UTF8.GetString(attempt as byte[]))
                : 0;
            set => Headers[nameof(RetryAttempt)] = Encoding.UTF8.GetBytes(value.ToString());
        }

        public string ReplyTo
        {
            get => Headers.TryGetValue(nameof(ReplyTo), out var value)
                ? Encoding.UTF8.GetString(value as byte[])
                : string.Empty;
            set => Headers[nameof(ReplyTo)] = Encoding.UTF8.GetBytes(value);
        }

        public MessageProperties Continue() =>
            new()
            {
                MessageId = MessageId is null ? null : $"{this.MessageId}",
                CorrelationId = this.CorrelationId,
                Headers = this.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                DeliveryMode = this.DeliveryMode,
                Expiration = this.Expiration,
                Persistent = this.Persistent
            };
    }
}