using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SafeBunny.Core.Pipeline
{
    internal sealed class Pipeline<TContext> : IPipeline<TContext> where TContext : IPipelineContext
    {
        private readonly IServiceScopeFactory _factory;

        private readonly List<Type> _coreSteps = new();
        private readonly List<Type> _preSteps = new();
        private readonly List<Type> _postSteps = new();

        public Pipeline(ILogger<Pipeline<TContext>> logger, IServiceScopeFactory factory)
        {
            _factory = factory;
        }

        public void RegisterCoreProcessor<T>() where T : ISafeBunnyMiddleware<TContext> => _coreSteps.Add(typeof(T));
        public void RegisterPreProcessor<T>() where T : ISafeBunnyMiddleware<TContext> => _preSteps.Add(typeof(T));
        public void RegisterPostProcessor<T>() where T : ISafeBunnyMiddleware<TContext> => _postSteps.Add(typeof(T));

        public async Task ProcessAsync(TContext context)
        {
            using var scope = _factory.CreateScope();
            context.Provider ??= scope.ServiceProvider;

            var steps = Steps();
            var funcs = new List<Func<Task>> {() => Task.CompletedTask};
            var instances = new List<object>();
            var methods = new List<MethodInfo>();

            for (int i = 0; i < steps.Count; i++)
            {
                var copy = i;
                instances.Add(context.Provider.GetRequiredService(steps[copy]));
                methods.Add(instances[copy].GetType().GetMethod(nameof(ISafeBunnyMiddleware<IPipelineContext>.InvokeAsync)));
                funcs.Add(() => (Task) methods[copy].Invoke(instances[copy], new object[] { context, funcs[copy] }));
            }

            await funcs.Last().Invoke().ConfigureAwait(false);
        }

        private List<Type> Steps() => _postSteps.Concat(_coreSteps).Concat(_preSteps).ToList();
    }
}