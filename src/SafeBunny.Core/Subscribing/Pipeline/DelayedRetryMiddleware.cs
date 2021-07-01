using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SafeBunny.Core.Pipeline;
using SafeBunny.Core.Publishing;

namespace SafeBunny.Core.Subscribing.Pipeline
{
    internal sealed class DelayedRetryMiddleware : ISafeBunnyMiddleware<IProcessingContext>
    {
        private readonly ILogger<DelayedRetryMiddleware> _logger;
        private readonly ISafeBunnyPublisher _publisher;

        private readonly MethodInfo _methodInfo = typeof(DelayedRetryMiddleware)
                .GetMethod(nameof(SendToDelayedAsync), BindingFlags.NonPublic | BindingFlags.Instance);

        public DelayedRetryMiddleware(ILogger<DelayedRetryMiddleware> logger, ISafeBunnyPublisher publisher)
        {
            _logger = logger;
            _publisher = publisher;
        }

        public async Task InvokeAsync(IProcessingContext context, Func<Task> next)
        {
            await RetryDelayedAsync(context).ConfigureAwait(false);
            await next().ConfigureAwait(false);
        }

        private Task RetryDelayedAsync(IProcessingContext context)
        {
            if (!context.ProcessingStatus.InMemoryRetryFailed || context.Metadata.BusRetry.Delay == TimeSpan.Zero)
            {
                return Task.CompletedTask;
            }

            if (context.Properties.RetryAttempt > context.Metadata.BusRetry.Count)
            {
                _logger.LogError("Maximum bus retry reached");
                context.ProcessingStatus.OnBusRetryFailed = true;
                return Task.CompletedTask;
            }

            context.Properties.RetryAttempt += 1;
            context.Properties.DeliveryDelay = context.Metadata.BusRetry.Delay;
            return (Task) _methodInfo
                .MakeGenericMethod(context.Metadata.MessageType)
                .Invoke(this, new object[] { context });
        }

        private Task SendToDelayedAsync<T>(IProcessingContext<T> context) where T : class
            => _publisher.PublishAsync(context.Message, context.Properties, context.Provider);
    }
}