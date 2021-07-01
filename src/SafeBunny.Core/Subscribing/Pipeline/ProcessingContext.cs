using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SafeBunny.Core.Message;
using SafeBunny.Core.Pipeline;
using SafeBunny.Core.Publishing;
using SafeBunny.Core.Subscribing.Topology.Subscription;
using SafeBunny.Core.Subscribing.Transactional;

namespace SafeBunny.Core.Subscribing.Pipeline
{
    internal sealed class ProcessingContext<T> : IProcessingContext<T>
    {
        public object Message { get; private set; }
        public MessageProperties Properties { get; private set; }
        public HandlerMetadata Metadata { get; private set; }

        private IServiceProvider _provider;
        private readonly ProcessingStatus _processingStatus;

        IServiceProvider IPipelineContext.Provider
        {
            get => _provider;
            set => _provider = value;
        }

        T IProcessingContext<T>.Message => (T) Message;
        ProcessingStatus IProcessingContext.ProcessingStatus => _processingStatus;

        public ProcessingContext(object message, MessageProperties properties, HandlerMetadata metadata)
        {
            Message = message;
            Properties = properties;
            Metadata = metadata;
            _processingStatus = new ProcessingStatus();
        }

        public async Task ContinueAsync<TOut>(TOut message) where TOut : class 
            => await Service<ISafeBunnyPublisher>().PublishAsync(message, Properties).ConfigureAwait(false);

        public async Task ReplyAsync<TOut>(TOut message) where TOut : class 
            => await Service<ISafeBunnyPublisher>().PublishAsync(message, Properties, Properties.ReplyTo).ConfigureAwait(false);

        public async Task TransactAsync(Func<Task> transaction) 
            => await Service<ITransactionalScope>().CommitAsync(Properties.CorrelationId, Properties.MessageId, transaction).ConfigureAwait(false);

        private TService Service<TService>() => _provider.GetRequiredService<TService>();
    }
}