using System;
using System.Threading.Tasks;
using SafeBunny.Core.Message;
using SafeBunny.Core.Pipeline;
using SafeBunny.Core.Subscribing.Pipeline;

namespace SafeBunny.Benchmarks.Reflection.SafeBunnyLazyReflectionDeps
{
    internal sealed class LazyExecution : ISafeBunnyMiddleware<IProcessingContext>
    {
        public async Task InvokeAsync(IProcessingContext context, Func<Task> next)
        {
            var service = context.Provider
                .GetService(typeof(IMessageHandler<>).MakeGenericType(context.Metadata.MessageType));
            await (service.GetType()
                .GetMethod(nameof(IMessageHandler<LazyExecution>.HandleAsync))
                .Invoke(service, new object[] {context}) as Task);
            await next();
        }
    }
}