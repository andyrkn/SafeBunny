using SafeBunny.Core.Subscribing.Topology;
using SafeBunny.Core.Subscribing.Topology.Subscription;
using SafeBunny.Core.Subscribing.Topology.Subscription.Retry;

namespace SafeBunny.Core.Subscribing.Builder
{
    internal sealed class SubscriptionBuilder : ISubscriptionBuilder
    {
        private readonly string _node;
        private readonly ITopology _topology;
        private readonly int _defaultQos;

        public SubscriptionBuilder(string node, ITopology topology, int defaultQos)
        {
            _node = node;
            _topology = topology;
            _defaultQos = defaultQos;
        }


        public ISubscriptionBuilder Consume<T>()
            => Consume<T>(_defaultQos);

        public ISubscriptionBuilder Consume<T>(int qos, RetryStrategy memoryRetry = null, RetryStrategy busRetry = null)
        {
            _topology.Register(_node, new HandlerMetadata(typeof(T), qos, memoryRetry, busRetry));
            return this;
        }
    }
}