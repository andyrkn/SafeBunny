using System;
using System.Reflection;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using SafeBunny.Benchmarks.Reflection.SafeBunnyLazyReflectionDeps;
using SafeBunny.Core.Extensions;
using SafeBunny.Core.Message;
using SafeBunny.Core.Pipeline;
using SafeBunny.Core.Subscribing.Pipeline;
using SafeBunny.Core.Subscribing.Topology.Subscription;
using SafeBunny.Core.Subscribing.Topology.Subscription.Retry;

namespace SafeBunny.Benchmarks.Reflection
{
    public class ReflectionBenchmarks
    {
        private IServiceProvider _lazyProvider;
        private IPipeline<IProcessingContext> _lazyPipeline;

        private IServiceProvider _cachedProvider;
        private IPipeline<IProcessingContext> _cachedPipeline;

        private readonly HandlerMetadata handlerMetadata = new HandlerMetadata(typeof(SomeMessageType), 10, RetryStrategy.None, RetryStrategy.None);

        public ReflectionBenchmarks()
        {
            RegisterLazy();
            RegisterCached();
        }

        private void RegisterLazy()
        {
            _lazyProvider = new ServiceCollection()
                .AddSingleton<IPipeline<IProcessingContext>, Pipeline<IProcessingContext>>()
                .AddLogging()
                .AddSafeBunnyMessageHandlers(typeof(Handler).Assembly)
                .BuildServiceProvider();

            _lazyPipeline = _lazyProvider.GetRequiredService<IPipeline<IProcessingContext>>();
            _lazyPipeline.RegisterPreProcessor<PreProcessing>();
            _lazyPipeline.RegisterCoreProcessor<LazyExecution>();
            _lazyPipeline.RegisterCoreProcessor<LazyRetry>();
            _lazyPipeline.RegisterPostProcessor<PostProcessing>();
        }

        private void RegisterCached()
        {
            _cachedProvider = new ServiceCollection()
                .AddSingleton<IPipeline<IProcessingContext>, Pipeline<IProcessingContext>>()
                .AddLogging()
                .AddSafeBunnyMessageHandlers(typeof(Handler).Assembly)
                .BuildServiceProvider();

            _cachedPipeline = _cachedProvider.GetRequiredService<IPipeline<IProcessingContext>>();
            _cachedPipeline.RegisterPreProcessor<PreProcessing>();
            _cachedPipeline.RegisterCoreProcessor<CachedExecution>();
            _cachedPipeline.RegisterCoreProcessor<CachedRetry>();
            _cachedPipeline.RegisterPostProcessor<PostProcessing>();
        }

        [Benchmark(Baseline = true)]
        public async Task RawReflection()
        {
            var context = typeof(ProcessingContext<>).MakeGenericType(typeof(SomeMessageType))
                .GetConstructor(BindingFlags.Instance | BindingFlags.Public,
                    null,
                    new[] {typeof(object), typeof(MessageProperties), typeof(HandlerMetadata)},
                    null)
                .Invoke(new object[] {null, null, handlerMetadata}) as IProcessingContext<SomeMessageType>;

            await _lazyPipeline.ProcessAsync(context);
        }

        [Benchmark]
        public async Task CachedReflection()
        {
            var context = handlerMetadata.ReflectionMetadata.NewContext(null, null, handlerMetadata);
            await _cachedPipeline.ProcessAsync(context);
        }
    }
}