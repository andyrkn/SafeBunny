namespace SafeBunny.Core.Subscribing.Topology
{
    internal sealed class SafeBunnyCircuitBreaker : ISafeBunnyCircuitBreaker
    {
        private readonly ITopologyConsumer _topology;

        public SafeBunnyCircuitBreaker(ITopologyConsumer topology)
        {
            _topology = topology;
        }

        public void Resume<T>() => _topology.Resume<T>();

        public void Cancel<T>() => _topology.Cancel<T>();
    }
}