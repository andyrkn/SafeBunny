using System.Threading.Tasks;
using SafeBunny.Core.Subscribing.Pipeline;

namespace SafeBunny.Core.Message
{
    public interface IMessageHandler<in T> where T:class
    {
        public Task HandleAsync(IProcessingContext<T> context);
    }
}