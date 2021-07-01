namespace SafeBunny.Core.Subscribing.Topology
{
    internal sealed class TopologyExchanges
    {
        public const string Delayed = "delayed-exchange";
        public const string Topic = nameof(SafeBunny);
        public static string ReplyTo = $"{nameof(SafeBunny)}.RPC";
    }
}