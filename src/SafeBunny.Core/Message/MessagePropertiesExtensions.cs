using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using RabbitMQ.Client;

namespace SafeBunny.Core.Message
{
    public static class MessagePropertiesExtensions
    {
        public static MessageProperties ToUserProps(this IBasicProperties props)
        {
            var p = new MessageProperties
            {
                MessageId = props.MessageId,
                CorrelationId = props.CorrelationId,
                Expiration = TimeSpan.FromMilliseconds(double.Parse(props.Expiration)),
                DeliveryMode = props.DeliveryMode == 1 ? DeliveryMode.NonPersistent : DeliveryMode.Persistent,
                Persistent = props.Persistent,
                Headers = (Dictionary<string, object>) props.Headers
            };

            return p;
        }

        internal static IBasicProperties ToBasicProps(this MessageProperties props, IBasicProperties p)
        {
            p.Headers = props.Headers;
            p.Expiration = props.Expiration.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
            p.CorrelationId = props.CorrelationId;
            p.MessageId = props.MessageId;
            p.DeliveryMode = props.DeliveryMode == DeliveryMode.NonPersistent ? (byte) 1 : (byte) 2;
            p.Persistent = props.Persistent;
            return p;
        }

        public static ulong GetDeliveryTag(this IBasicProperties props)
        { 
            if (props.Headers.TryGetValue(SafeBunnyDeliveryTag, out var value))
            {
                ulong.TryParse(Encoding.UTF8.GetString(value as byte[]), out var result);
                return result;
            }

            return 0;
        }

        public static void SetDeliveryTag(this IBasicProperties props, ulong deliveryTag)
        {
            props.Headers[SafeBunnyDeliveryTag] = Encoding.UTF8.GetBytes(deliveryTag.ToString());
        }

        private const string SafeBunnyDeliveryTag = nameof(SafeBunnyDeliveryTag);
    }
}