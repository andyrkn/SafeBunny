using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SafeBunny.Core.Pipeline;

namespace SafeBunny.Benchmarks.Pipeline
{
    internal sealed class ActivatorPipeline<TContext> : IPipeline<TContext> where TContext : IPipelineContext
    {
        private readonly IServiceProvider _provider;

        private readonly List<Type> _coreSteps = new();
        private readonly List<Type> _preSteps = new();
        private readonly List<Type> _postSteps = new();

        public ActivatorPipeline(ILogger<Pipeline<TContext>> logger, IServiceProvider provider)
        {
            _provider = provider;
        }

        public void RegisterCoreProcessor<T>() where T : ISafeBunnyMiddleware<TContext> => _coreSteps.Add(typeof(T));
        public void RegisterPreProcessor<T>() where T : ISafeBunnyMiddleware<TContext> => _preSteps.Add(typeof(T));
        public void RegisterPostProcessor<T>() where T : ISafeBunnyMiddleware<TContext> => _postSteps.Add(typeof(T));

        public async Task ProcessAsync(TContext context)
        {
            using var scope = _provider.CreateScope();
            context.Provider = scope.ServiceProvider;

            var steps = Steps();
            var funcs = new List<Func<Task>> {() => Task.CompletedTask};
            var instances = new List<object>();
            var methods = new List<MethodInfo>();

            for (int i = 0; i < steps.Count; i++)
            {
                var copy = i;
                instances.Add(ActivatorUtilities.CreateInstance(context.Provider, steps[copy]));
                methods.Add(instances[copy].GetType().GetMethod(nameof(ISafeBunnyMiddleware<IPipelineContext>.InvokeAsync)));
                funcs.Add(() => (Task) methods[copy].Invoke(instances[copy], new object[] { context, funcs[copy] }));
            }

            await funcs.Last().Invoke();
        }

        private List<Type> Steps() => _postSteps.Concat(_coreSteps).Concat(_preSteps).ToList();
    }
}