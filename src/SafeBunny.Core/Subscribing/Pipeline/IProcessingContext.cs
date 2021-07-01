using System;
using System.Threading.Tasks;
using SafeBunny.Core.Message;
using SafeBunny.Core.Pipeline;
using SafeBunny.Core.Subscribing.Topology.Subscription;

namespace SafeBunny.Core.Subscribing.Pipeline
{
    public interface IProcessingContext : IPipelineContext
    {
        public object Message { get; }
        public MessageProperties Properties { get; }
        public HandlerMetadata Metadata { get; }
        internal ProcessingStatus ProcessingStatus { get; }

        public Task ContinueAsync<TOut>(TOut message) where TOut : class;
        public Task ReplyAsync<TOut>(TOut message) where TOut : class;
        public Task TransactAsync(Func<Task> transaction);
    }

    public interface IProcessingContext<out TIn> : IProcessingContext
    {
        public new TIn Message { get; }
    }
}