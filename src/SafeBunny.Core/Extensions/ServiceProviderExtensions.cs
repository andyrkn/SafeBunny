using System;
using Microsoft.Extensions.DependencyInjection;
using SafeBunny.Core.Pipeline;
using SafeBunny.Core.Publishing.Pipeline;
using SafeBunny.Core.Subscribing.Builder;
using SafeBunny.Core.Subscribing.Pipeline;
using SafeBunny.Core.Subscribing.Topology;
using SafeBunny.Core.Subscribing.Topology.Inspection;

namespace SafeBunny.Core.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static IServiceProvider UseSafeBunnySubscriptions(this IServiceProvider provider, Action<ISubscriptionManager> act)
        {
            using var scope = provider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ISubscriptionManager>();
            act(service);

            return provider;
        }

        public static IServiceProvider UseSafeBunnyPreProcessor<T>(this IServiceProvider provider)
            where T : ISafeBunnyMiddleware<IProcessingContext>
        {
            var pipeline = provider.GetRequiredService<IPipeline<IProcessingContext>>();
            pipeline.RegisterPreProcessor<T>();

            return provider;
        }

        public static IServiceProvider UseSafeBunnyPostProcessor<T>(this IServiceProvider provider)
            where T : ISafeBunnyMiddleware<IProcessingContext>
        {
            var pipeline = provider.GetRequiredService<IPipeline<IProcessingContext>>();
            pipeline.RegisterPostProcessor<T>();

            return provider;
        }

        public static IServiceProvider UseSafeBunnyPrePublisher<T>(this IServiceProvider provider)
            where T : ISafeBunnyMiddleware<IPublishingContext>
        {
            var pipeline = provider.GetRequiredService<IPipeline<IPublishingContext>>();
            pipeline.RegisterPreProcessor<T>();

            return provider;
        }

        public static IServiceProvider UseSafeBunnyPostPublisher<T>(this IServiceProvider provider)
            where T : ISafeBunnyMiddleware<IPublishingContext>
        {
            var pipeline = provider.GetRequiredService<IPipeline<IPublishingContext>>();
            pipeline.RegisterPostProcessor<T>();

            return provider;
        }

        public static IServiceProvider UseSafeBunny(this IServiceProvider provider)
        {
            provider.GetRequiredService<ITopologyInspector>().Inspect();

            var publishingPipeline = provider.GetRequiredService<IPipeline<IPublishingContext>>();
            publishingPipeline.RegisterCoreProcessor<PublishingMiddleware>();

            var consumingPipeline = provider.GetRequiredService<IPipeline<IProcessingContext>>();
            consumingPipeline.RegisterCoreProcessor<ExecutionMiddleware>();
            consumingPipeline.RegisterCoreProcessor<DelayedRetryMiddleware>();

            var consumer = provider.GetRequiredService<ITopologyConsumer>();
            consumer.Start();
            return provider;
        }
    }
}