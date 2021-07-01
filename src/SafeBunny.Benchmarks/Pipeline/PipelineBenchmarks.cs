using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using SafeBunny.Benchmarks.SafeBunnyDeps;
using SafeBunny.Core.Extensions;
using SafeBunny.Core.Pipeline;
using SafeBunny.Core.Subscribing.Pipeline;

namespace SafeBunny.Benchmarks.Pipeline
{
    public class PipelineBenchmarks
    {
        private IServiceProvider _scopedProvider;
        private IPipeline<IProcessingContext> _scopedPipeline;

        private IServiceProvider _unregisteredProvider;
        private IPipeline<IProcessingContext> _unregisteredPipeline;

        private IServiceProvider _singletonProvider;
        private IPipeline<IProcessingContext> _singletonPipeline;

        public PipelineBenchmarks()
        {
            RegisterScoped();
            RegisterSingletons();
            RegisterUnregisteredPipeline();
        }

        private void RegisterSingletons()
        {
            _singletonProvider = new ServiceCollection()
                .AddSingleton<IPipeline<IProcessingContext>, Pipeline<IProcessingContext>>()
                .AddLogging()
                .AddSafeBunnyMessageHandlersAsSingleton(typeof(Program).Assembly)
                .BuildServiceProvider();

            _singletonPipeline = _singletonProvider.GetRequiredService<IPipeline<IProcessingContext>>();
            _singletonPipeline.RegisterPreProcessor<PreProcessing>();
            _singletonPipeline.RegisterCoreProcessor<Execution>();
            _singletonPipeline.RegisterCoreProcessor<Retry>();
            _singletonPipeline.RegisterPostProcessor<PostProcessing>();
        }

        private void RegisterScoped()
        {
            _scopedProvider = new ServiceCollection()
                .AddSingleton<IPipeline<IProcessingContext>, Pipeline<IProcessingContext>>()
                .AddLogging()
                .AddSafeBunnyMessageHandlers(typeof(Program).Assembly)
                .BuildServiceProvider();

            _scopedPipeline = _scopedProvider.GetRequiredService<IPipeline<IProcessingContext>>();
            _scopedPipeline.RegisterPreProcessor<PreProcessing>();
            _scopedPipeline.RegisterCoreProcessor<Execution>();
            _scopedPipeline.RegisterCoreProcessor<Retry>();
            _scopedPipeline.RegisterPostProcessor<PostProcessing>();
        }

        private void RegisterUnregisteredPipeline()
        {
            _unregisteredProvider = new ServiceCollection()
                .AddSingleton<IPipeline<IProcessingContext>, ActivatorPipeline<IProcessingContext>>()
                .AddLogging()
                .AddSafeBunnyMessageHandlers(typeof(Program).Assembly)
                .BuildServiceProvider();

            _unregisteredPipeline = _unregisteredProvider.GetRequiredService<IPipeline<IProcessingContext>>();
            _unregisteredPipeline.RegisterPreProcessor<PreProcessing>();
            _unregisteredPipeline.RegisterCoreProcessor<Execution>();
            _unregisteredPipeline.RegisterCoreProcessor<Retry>();
            _unregisteredPipeline.RegisterPostProcessor<PostProcessing>();
        }

        [Benchmark(Baseline = true)]
        public async Task Scoped()
        {
            await _scopedPipeline.ProcessAsync(new ProcessingContext<SomeMessageType>(new SomeMessageType(), null, null));
        }

        [Benchmark]
        public async Task ScopedActivator()
        {
            await _unregisteredPipeline.ProcessAsync(new ProcessingContext<SomeMessageType>(new SomeMessageType(), null, null));
        }

        [Benchmark]
        public async Task Singleton()
        {
            await _singletonPipeline.ProcessAsync(new ProcessingContext<SomeMessageType>(new SomeMessageType(), null, null));
        }
    }
}