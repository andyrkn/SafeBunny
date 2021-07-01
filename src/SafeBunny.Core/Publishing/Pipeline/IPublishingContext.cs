using System;
using SafeBunny.Core.Message;
using SafeBunny.Core.Pipeline;

namespace SafeBunny.Core.Publishing.Pipeline
{
    public interface IPublishingContext : IPipelineContext
    {
        object Message { get; }
        Type MessageType { get; }
        MessageProperties Properties { get; }
        string ReplyTo { get; }
    }

    public interface IPublishingContext<out T>:IPublishingContext where T: class
    {
        new T Message { get; }
    }
}