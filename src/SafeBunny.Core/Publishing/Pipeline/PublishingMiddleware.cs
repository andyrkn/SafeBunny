using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SafeBunny.Core.Message;
using SafeBunny.Core.Pipeline;
using SafeBunny.Core.Publishing.Continuation;
using SafeBunny.Core.Publishing.Infrastructure;
using SafeBunny.Core.Publishing.Scheduler;
using SafeBunny.Core.Serialization;
using SafeBunny.Core.Subscribing.Topology;

namespace SafeBunny.Core.Publishing.Pipeline
{
    internal sealed class PublishingMiddleware : ISafeBunnyMiddleware<IPublishingContext>
    {
        private readonly ICorrelationContinuation _correlationContinuation;
        private readonly ISafeBunnySerializer _serializer;
        private readonly IPublishScheduler _scheduler;
        private readonly IPublishChannelsContainer _container;
        private readonly SafeBunnySettings _settings;

        public PublishingMiddleware
            (ICorrelationContinuation correlationContinuation, 
            ISafeBunnySerializer serializer, 
            IPublishScheduler scheduler, 
            IPublishChannelsContainer container, IOptions<SafeBunnySettings> settings)
        {
            _correlationContinuation = correlationContinuation;
            _serializer = serializer;
            _scheduler = scheduler;
            _container = container;
            _settings = settings.Value;
        }

        public async Task InvokeAsync(IPublishingContext context, Func<Task> next)
        {
            var exchange =
                GetExchange(
                    string.IsNullOrEmpty(context.ReplyTo) ? TopologyExchanges.Topic : TopologyExchanges.ReplyTo,
                    context.Properties.DeliveryDelay);
            await PublishAsync(context, exchange).ConfigureAwait(false);
        }

        private async Task PublishAsync(IPublishingContext context, string exchange)
        {
            var props = context.Properties.Continue();
            props.ContinuationMarker = props.ContinuationMarker + 1;
            props.MessageId = _correlationContinuation.Continue(props.CorrelationId, props.ContinuationMarker);

            var body = _serializer.Serialize(context.Message, context.MessageType);

            await _scheduler.ScheduleAsync(async () =>
            {
                var channel = await _container.Get().ConfigureAwait(false);
                try
                {
                    var basicProps = props.ToBasicProps(channel.Channel.CreateBasicProperties());
                    basicProps.SetDeliveryTag(channel.Channel.NextPublishSeqNo);
                    await channel
                        .PublishAsync(exchange, $"{_settings.Node}.{context.MessageType.Name}", body, basicProps)
                        .ConfigureAwait(false);
                }
                finally
                {
                    _container.Return(channel);
                }
            }).ConfigureAwait(false);
        }

        private string GetExchange(string exchange, TimeSpan delay)
            => delay.TotalMilliseconds == 0
                ? exchange
                : TopologyExchanges.Delayed;
    }
}