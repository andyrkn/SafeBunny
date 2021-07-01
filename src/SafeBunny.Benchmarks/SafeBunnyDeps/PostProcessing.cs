using System;
using System.Threading.Tasks;
using SafeBunny.Core.Pipeline;
using SafeBunny.Core.Subscribing.Pipeline;

namespace SafeBunny.Benchmarks.SafeBunnyDeps
{
    public class PostProcessing : ISafeBunnyMiddleware<IProcessingContext>
    {
        public Task InvokeAsync(IProcessingContext context, Func<Task> next)
            => next();
    }
}