using System;

namespace SafeBunny.Core.Subscribing.Topology.Subscription.Retry
{
    public sealed class RetryStrategy
    {
        public readonly int Count;
        public readonly TimeSpan Delay;

        public RetryStrategy(int count, TimeSpan delay)
        {
            Count = count;
            Delay = delay;
        }

        public static RetryStrategy From(int count, TimeSpan delay) => new (count, delay);
        /// <summary>
        /// Defaults to 5 in memory retries at 100ms
        /// </summary>
        public static RetryStrategy Default => new RetryStrategy(5, TimeSpan.FromMilliseconds(100));
        public static RetryStrategy None => new(0, TimeSpan.Zero);
    }
}