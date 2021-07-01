using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SafeBunny.Core.Connection;
using SafeBunny.Core.Message;
using SafeBunny.Core.Pipeline;
using SafeBunny.Core.Publishing;
using SafeBunny.Core.Publishing.Continuation;
using SafeBunny.Core.Publishing.Infrastructure;
using SafeBunny.Core.Publishing.Pipeline;
using SafeBunny.Core.Publishing.Scheduler;
using SafeBunny.Core.Serialization;
using SafeBunny.Core.Subscribing.Builder;
using SafeBunny.Core.Subscribing.Pipeline;
using SafeBunny.Core.Subscribing.Topology;
using SafeBunny.Core.Subscribing.Topology.Inspection;

namespace SafeBunny.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private const string BunnyConnection = "SafeBunny";

        public static IServiceCollection AddSafeBunny(this IServiceCollection services, IConfiguration configuration)
            => services
                .Configure<SafeBunnySettings>(o => configuration.GetSection(BunnyConnection).Bind(o))
                .AddScopeds()
                .AddSingletons()
                .AddSafeBunnyMessageHandlers(typeof(SafeBunnySettings).Assembly);

        private static IServiceCollection AddSingletons(this IServiceCollection services)
            => services
                .AddSingleton<ICorrelationContinuation, CorrelationContinuation>()
                .AddSingleton<ISafeBunnySerializer, JsonSerializer>()
                .AddSingleton<ITopology, Topology>()
                .AddSingleton<ITopologyConsumer, TopologyConsumer>()
                .AddSingleton<IPublishScheduler, PublishScheduler>()
                .AddSingleton<IPublishChannelsContainer, PublishChannelsContainer>()
                .AddSingleton<IPipeline<IProcessingContext>, Pipeline<IProcessingContext>>()
                .AddSingleton<IPipeline<IPublishingContext>, Pipeline<IPublishingContext>>()
                .AddSingleton<ISafeBunnyConnectionFactory, SafeBunnyConnectionFactory>()
                .AddSingleton<ISafeBunnyCircuitBreaker, SafeBunnyCircuitBreaker>()
                .AddSingleton<ISafeBunnyPublisher, SafeBunnyPublisher>();

        private static IServiceCollection AddScopeds(this IServiceCollection services)
            => services
                .AddTransient<ITopologyInspector, TopologyInspector>()
                .AddScoped<ISubscriptionManager, SubscriptionManager>();

        public static IServiceCollection AddSafeBunnyMessageHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            var handlers = new List<Type> { typeof(IMessageHandler<>), typeof(ISafeBunnyMiddleware<>) };

            assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => type
                    .GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .Any(i => handlers.Contains(i.GetGenericTypeDefinition())))
                .ToList()
                .ForEach(implementation => implementation
                    .GetInterfaces()
                    .Where(handlerInterface => handlers.Contains(handlerInterface.GetGenericTypeDefinition()))
                    .ToList().ForEach(handlerInterface =>
                    {
                        var interfaceGeneric = handlerInterface.GetGenericTypeDefinition();
                        var type = handlerInterface.GetGenericArguments().First();
                        var interfaceType = interfaceGeneric.MakeGenericType(type); 
                        
                        if (interfaceGeneric == typeof(ISafeBunnyMiddleware<>))
                        {
                            services.AddScoped(implementation);
                        }
                        else
                        {
                            services.AddScoped(interfaceType, implementation);
                        }
                    }));

            return services;
        }
    }
}