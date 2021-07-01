using System;
using System.Reflection;
using System.Threading.Tasks;
using SafeBunny.Core.Pipeline;
using SafeBunny.Core.Subscribing.Pipeline;

namespace SafeBunny.Benchmarks.Reflection.SafeBunnyLazyReflectionDeps
{
    public class LazyRetry : ISafeBunnyMiddleware<IProcessingContext>
    {
        public async Task InvokeAsync(IProcessingContext context, Func<Task> next)
        {
            await (typeof(LazyRetry)
                .GetMethod(nameof(SendToDelayedAsync), BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(context.Metadata.MessageType)
                .Invoke(this, new object[] {context}) as Task);
            await next();
        }
        Task SendToDelayedAsync<T>(IProcessingContext<T> context) => Task.CompletedTask;
    }
}