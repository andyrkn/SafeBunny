using System;
using System.Threading.Tasks;
using SafeBunny.Core.Message;

namespace SafeBunny.Core.Publishing
{
    public interface ISafeBunnyPublisher
    {
        Task PublishAsync<T>(T stuff, MessageProperties props) where T : class;
        internal Task PublishAsync<T>(T stuff, MessageProperties props, string replyTo) where T : class;
        internal Task PublishAsync<T>(T stuff, MessageProperties props, IServiceProvider provider) where T : class;
        internal Task PublishAsync<T>(T stuff, MessageProperties props, string replyTo, IServiceProvider provider) where T : class;
    }
}