using System;
using SafeBunny.Core.Message;
using SafeBunny.Core.Pipeline;

namespace SafeBunny.Core.Publishing.Pipeline
{
    internal sealed class PublishingContext<T> : IPublishingContext<T> where T : class
    {
        private IServiceProvider _provider;
        private readonly object _message;

        public Type MessageType { get; private set; }
        public MessageProperties Properties { get; private set; }
        public string ReplyTo { get; private set; }

        public PublishingContext(T message, MessageProperties props, string replyTo)
        {
            _message = message;
            Properties = props;
            ReplyTo = replyTo;
            MessageType = typeof(T);
        }

        public PublishingContext(T message, MessageProperties props, string replyTo, IServiceProvider provider)
        {
            _message = message;
            Properties = props;
            ReplyTo = replyTo;
            MessageType = typeof(T);
            _provider = provider;
        }

        public T Message => (T) _message;
        object IPublishingContext.Message => _message;

        IServiceProvider IPipelineContext.Provider
        {
            get => _provider;
            set => _provider = value;
        }
    }
}