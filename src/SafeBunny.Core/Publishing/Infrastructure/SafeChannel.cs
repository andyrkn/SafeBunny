using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SafeBunny.Core.Message;

namespace SafeBunny.Core.Publishing.Infrastructure
{
    internal sealed class SafeChannel : IDisposable
    {
        public IModel Channel { get; }

        private readonly ConcurrentDictionary<ulong, TaskCompletionSource<bool>> _inFlightMessages;
        private readonly ILogger<PublishChannelsContainer> _logger;

        public SafeChannel(IConnection connection, ILogger<PublishChannelsContainer> logger)
        {
            _logger = logger;
            Channel = connection.CreateModel();
            Channel.ConfirmSelect();
            _inFlightMessages = new ConcurrentDictionary<ulong, TaskCompletionSource<bool>>();

            Channel.BasicAcks += ChannelOnBasicAcks;
            Channel.BasicNacks += ChannelOnBasicNacks;
            Channel.BasicReturn += ChannelOnBasicReturn;
            Channel.ModelShutdown += (sender, args) => { _logger.LogWarning($"Shutdown model {args.ReplyCode}-{args.ReplyText}"); };
        }

        private void ChannelOnBasicReturn(object sender, BasicReturnEventArgs e)
        {
            var error = $"channel {Channel.ChannelNumber}-Message routed to {e.RoutingKey}, returned: {e.ReplyCode}-{e.ReplyText}";
            _logger.LogError(error);
            
            if (_inFlightMessages.TryRemove(e.BasicProperties.GetDeliveryTag(), out var tcs))
            {
                tcs.SetException(new RoutingException(error));
            }
        }

        private void ChannelOnBasicNacks(object sender, BasicNackEventArgs e)
        {
            if (e.Multiple)
            {
                SetMultipleInFlight(e.DeliveryTag, false);
            }
            else
            {
                SetInFlight(e.DeliveryTag, false);
            }
        }

        private void ChannelOnBasicAcks(object sender, BasicAckEventArgs e)
        {
            if (e.Multiple)
            {
                SetMultipleInFlight(e.DeliveryTag, true);
            }
            else
            {
                SetInFlight(e.DeliveryTag, true);
            }
        }

        public Task<bool> PublishAsync(string exchange, string routingKey, byte[] body, IBasicProperties props)
        {
            var tcs = new TaskCompletionSource<bool>();
            _inFlightMessages.TryAdd(props.GetDeliveryTag(), tcs);

            Channel.BasicPublish(exchange, routingKey, true, props, body);

            return tcs.Task;
        }

        private void SetMultipleInFlight(ulong deliveryTag, bool result)
        {
            foreach (var tag in _inFlightMessages.Keys)
            {
                if (deliveryTag <= tag)
                {
                    SetInFlight(tag, result);
                }
            }
        }

        private void SetInFlight(ulong deliveryTag, bool result)
        {
            if (_inFlightMessages.TryRemove(deliveryTag, out var tcs))
            {
                if (result) 
                     tcs.SetResult(true);
                else
                    tcs.SetException(new RoutingException($"Message {deliveryTag} did not reach the broker"));
            }
        }

        public void Dispose()
        {
            Channel?.Close();
            Channel?.Dispose();
        }
    }
}