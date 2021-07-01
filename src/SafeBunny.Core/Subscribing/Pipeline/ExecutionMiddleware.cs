using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using SafeBunny.Core.Pipeline;

namespace SafeBunny.Core.Subscribing.Pipeline
{
    internal sealed class ExecutionMiddleware : ISafeBunnyMiddleware<IProcessingContext>
    {
        private readonly ILogger<ExecutionMiddleware> _logger;

        public ExecutionMiddleware(ILogger<ExecutionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(IProcessingContext context, Func<Task> next)
        {
            var instance = context.Provider.GetRequiredService(context.Metadata.ReflectionMetadata.HandlerType);

            try
            {
                await Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(
                        context.Metadata.MemoryRetry.Count, 
                        _ => context.Metadata.MemoryRetry.Delay,
                        (ex,_, retry, __) => _logger.LogError($"RetryAttempt {retry}:{ex.InnerException?.Message ?? ex.Message}"))
                    .ExecuteAsync(() => 
                        (Task) context.Metadata.ReflectionMetadata.MethodInfo.Invoke(instance, new object[] {context}));
            }
            catch (Exception ex)
            {
                if (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                _logger.LogError($"Failed to execute: {ex.StackTrace}");
                context.ProcessingStatus.InMemoryRetryFailed = true;
            }

            await next();
        }
    }
}