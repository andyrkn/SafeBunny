using SafeBunny.Core.Subscribing.Topology.Subscription.Retry;

namespace SafeBunny.Core.Subscribing.Builder
{
    public interface ISubscriptionBuilder
    {
        public ISubscriptionBuilder Consume<T>();

        public ISubscriptionBuilder Consume<T>(int qos, RetryStrategy memoryRetry = null, RetryStrategy busRetry = null);
    }
}