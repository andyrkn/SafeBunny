using System;
using System.Collections.Generic;
using SafeBunny.Core.Subscribing.Topology.Subscription;

namespace SafeBunny.Core.Subscribing.Topology
{
    internal interface ITopology
    {
        public void Register(string node);
        public void Register(string node, HandlerMetadata handler);

        public HandlerMetadata FirstOrDefault(Type type);

        public List<(string node, List<HandlerMetadata> handlers)> Subscriptions();
    }
}