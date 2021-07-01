using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SafeBunny.Core.Message;
using SafeBunny.Core.Pipeline;

namespace SafeBunny.Benchmarks.Pipeline
{
    public static class SignletonExtensions
    {
        internal static IServiceCollection AddSafeBunnyMessageHandlersAsSingleton(this IServiceCollection services, params Assembly[] assemblies)
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
                            services.AddSingleton(implementation);
                        }
                        else
                        {
                            services.AddSingleton(interfaceType, implementation);
                        }
                    }));

            return services;
        }
    }
}