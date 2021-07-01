using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using SafeBunny.Core.Subscribing.Topology.Subscription;

namespace SafeBunny.Core.Subscribing.Topology
{
    internal sealed class Topology : ITopology
    {
        private readonly SafeBunnySettings _settings;
        private readonly Dictionary<string, SubscriptionMetadata> _subscriptions = new();

        public Topology(IOptions<SafeBunnySettings> settings)
        {
            _settings = settings.Value;
        }

        public void Register(string node)
        {
            _subscriptions.Add(node, new());
        }

        public void Register(string node, HandlerMetadata handler)
        {
            if(_subscriptions.TryGetValue(node, out var subscription))
            {
                subscription.Handlers.Add(handler);
            }
        }

        public HandlerMetadata FirstOrDefault(Type type)
            => _subscriptions
                .SelectMany(pair => pair.Value.Handlers)
                .FirstOrDefault(h => h.MessageType == type);

        public List<(string node, List<HandlerMetadata> handlers)> Subscriptions()
            => _subscriptions
                .Select(kvp => (kvp.Key, kvp.Value.Handlers.Select(h => h).ToList()))
                .ToList();
    }
}