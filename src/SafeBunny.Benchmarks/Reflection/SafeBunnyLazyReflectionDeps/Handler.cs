using System.Threading.Tasks;
using SafeBunny.Core.Message;
using SafeBunny.Core.Subscribing.Pipeline;

namespace SafeBunny.Benchmarks.Reflection.SafeBunnyLazyReflectionDeps
{
    internal sealed class Handler : IMessageHandler<SomeMessageType>
    {
        public Task HandleAsync(IProcessingContext<SomeMessageType> context)
        {
            return Task.CompletedTask;
        }
    }
}