using System;
using System.Threading.Tasks;
using SafeBunny.Core.Message;
using SafeBunny.Core.Pipeline;
using SafeBunny.Core.Publishing.Pipeline;

namespace SafeBunny.Core.Publishing
{
    internal sealed class SafeBunnyPublisher : ISafeBunnyPublisher
    {
        private readonly IPipeline<IPublishingContext> _pipeline;

        public SafeBunnyPublisher(IPipeline<IPublishingContext> pipeline)
        {
            _pipeline = pipeline;
        }

        public async Task PublishAsync<T>(T stuff, MessageProperties props) where T : class
        {
            var context = new PublishingContext<T>(stuff, props, string.Empty);
            await _pipeline.ProcessAsync(context).ConfigureAwait(false);
        }

        async Task ISafeBunnyPublisher.PublishAsync<T>(T stuff, MessageProperties props, string replyTo) where T : class
        {
            var context = new PublishingContext<T>(stuff, props, replyTo);
            await _pipeline.ProcessAsync(context).ConfigureAwait(false); ;
        }

        async Task ISafeBunnyPublisher.PublishAsync<T>(T stuff, MessageProperties props, IServiceProvider provider) where T : class
        {
            var context = new PublishingContext<T>(stuff, props, string.Empty, provider);
            await _pipeline.ProcessAsync(context).ConfigureAwait(false); ;
        }

        async Task ISafeBunnyPublisher.PublishAsync<T>(T stuff, MessageProperties props, string replyTo, IServiceProvider provider) where T : class
        {
            var context = new PublishingContext<T>(stuff, props, replyTo, provider);
            await _pipeline.ProcessAsync(context).ConfigureAwait(false); ;
        }
    }
}