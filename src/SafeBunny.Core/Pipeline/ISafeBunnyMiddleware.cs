using System;
using System.Threading.Tasks;

namespace SafeBunny.Core.Pipeline
{
    public interface ISafeBunnyMiddleware<in TContext> where TContext : IPipelineContext
    {
        public Task InvokeAsync(TContext context, Func<Task> next);
    }
}