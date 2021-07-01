using System;
using System.Reflection;
using System.Threading.Tasks;
using SafeBunny.Core.Pipeline;
using SafeBunny.Core.Subscribing.Pipeline;

namespace SafeBunny.Benchmarks.Reflection.SafeBunnyLazyReflectionDeps
{
    public class CachedRetry : ISafeBunnyMiddleware<IProcessingContext>
    {
        private MethodInfo methodinfo = typeof(CachedRetry)
            .GetMethod(nameof(SendToDelayedAsync), BindingFlags.NonPublic | BindingFlags.Instance);

        public async Task InvokeAsync(IProcessingContext context, Func<Task> next)
        {
            methodinfo
                .MakeGenericMethod(context.Metadata.MessageType)
                .Invoke(this, new object[] {context});
            await next();
        }
        Task SendToDelayedAsync<T>(IProcessingContext<T> context) => Task.CompletedTask;
    }
}