namespace SafeBunny.Core.Subscribing.Topology
{
    internal interface ITopologyConsumer
    {
        public void Start();
        public void Resume<T>();
        public void Cancel<T>();
    }
}