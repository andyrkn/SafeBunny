using RabbitMQ.Client;

namespace SafeBunny.Core.Connection
{
    internal interface ISafeBunnyConnectionFactory
    {
        IConnection CreateConnection(string name);
    }
}