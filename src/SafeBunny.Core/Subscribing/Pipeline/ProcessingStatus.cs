namespace SafeBunny.Core.Subscribing.Pipeline
{
    internal sealed class ProcessingStatus
    {
        public bool InMemoryRetryFailed { get; set; }
        public bool OnBusRetryFailed { get; set; }
    }
}