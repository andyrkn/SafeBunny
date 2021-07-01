using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SafeBunny.Core.Pipeline;
using SafeBunny.Core.Subscribing.Pipeline;

namespace SafeBunny.Benchmarks.Reflection.SafeBunnyLazyReflectionDeps
{
    internal sealed class CachedExecution : ISafeBunnyMiddleware<IProcessingContext>
    {
        public async Task InvokeAsync(IProcessingContext context, Func<Task> next)
        {
            var instance = context.Provider.GetRequiredService(context.Metadata.ReflectionMetadata.HandlerType);
            await (Task) context.Metadata.ReflectionMetadata.MethodInfo.Invoke(instance, new object[] { context });
            await next();
        }
    }
}