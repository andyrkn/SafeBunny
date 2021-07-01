namespace SafeBunny.Core.Subscribing.Topology
{
    public interface ISafeBunnyCircuitBreaker
    {
        public void Resume<T>();
        public void Cancel<T>();
    }
}