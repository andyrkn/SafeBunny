namespace SafeBunny.Core
{
    public sealed class SafeBunnySettings
    {
        public string HostName { get; set; } = string.Empty;
        public int Port { get; set; } = 5672;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public ushort PrefetchCount { get; set; } = 1;
        public int RpcTimeout { get; set; } = 30;
        public int BasicQoS { get; set; } = 10;
        public string Node { get; set; }
        public int MaximumPublishConcurrency { get; set; } = 8;
    }
}