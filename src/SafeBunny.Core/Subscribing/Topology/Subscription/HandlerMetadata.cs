using System;
using SafeBunny.Core.Subscribing.Topology.Subscription.Retry;

namespace SafeBunny.Core.Subscribing.Topology.Subscription
{
    public sealed class HandlerMetadata
    {
        public readonly int QoS;
        public readonly Type MessageType;
        public readonly RetryStrategy MemoryRetry;
        public readonly RetryStrategy BusRetry;
        internal readonly ReflectionMetadata ReflectionMetadata;

        public HandlerMetadata(Type type, int qos, RetryStrategy memoryRetry, RetryStrategy busRetry)
        {
            QoS = qos;
            MessageType = type;
            MemoryRetry = memoryRetry ?? RetryStrategy.Default;
            BusRetry = busRetry ?? RetryStrategy.None;
            ReflectionMetadata = new ReflectionMetadata(type);
        }
    }
}