using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SafeBunny.Core.Subscribing.Topology.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SafeBunny.Core.Subscribing.Topology.Inspection
{
    internal sealed class TopologyInspector : ITopologyInspector
    {
        private readonly ITopology _topology;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TopologyInspector> _logger;

        public TopologyInspector(ITopology topology, IServiceProvider serviceProvider, ILogger<TopologyInspector> logger)
        {
            _topology = topology;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public void Inspect()
        {
            var errors = new List<Type>();
            var innerException = new Exception();
            _topology.Subscriptions().ForEach(sub =>
            {
                sub.handlers.ForEach(handler =>
                {
                    try
                    {
                        _serviceProvider.GetRequiredService(handler.ReflectionMetadata.HandlerType);
                    }
                    catch(Exception ex)
                    {
                        errors.Add(handler.MessageType);
                        innerException = new Exception(ex.Message, ex);
                    }
                });
            });

            if (errors.Any())
            {
                var error = $"Unable to activate: {string.Join(", ", errors.Select(t => t.Name))}";
                _logger.LogCritical("Unable to activate handlers for SafeBunny, see inner exception. Aborting");
                throw new ConsumerException(error, innerException);
            }
        }
    }
}