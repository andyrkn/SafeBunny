using System.Collections.Generic;

namespace SafeBunny.Core.Subscribing.Topology.Subscription
{ 
    internal sealed class SubscriptionMetadata
    {
        public readonly List<HandlerMetadata> Handlers;

        public SubscriptionMetadata()
        {
            Handlers = new();
        }
    }
}