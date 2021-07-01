using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using SafeBunny.Core.Extensions.Exceptions;

namespace SafeBunny.Core.Connection
{
    internal sealed class SafeBunnyConnectionFactory : ISafeBunnyConnectionFactory
    {
        private readonly SafeBunnySettings _settings;
        private readonly ILogger<SafeBunnyConnectionFactory> _logger;

        public SafeBunnyConnectionFactory(IOptions<SafeBunnySettings> settings, ILogger<SafeBunnyConnectionFactory> logger)
        {
            _settings = settings.Value;
            _logger = logger;
            if (settings.Value is null)
            {
                throw new ConfigurationException("The IConfiguration settings for SafeBunny are null");
            }
        }

        public IConnection CreateConnection(string name)
        {   
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.Username,
                Password = _settings.Password,
                Port = _settings.Port,
                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                AutomaticRecoveryEnabled = true,
                DispatchConsumersAsync = true
            };

            return Policy.Handle<Exception>()
                .WaitAndRetryForever(
                    _ => TimeSpan.FromSeconds(10),
                    (exception, _, span) => _logger.LogError($"Connection {name} failed. Retrying in {span.TotalSeconds}\n{exception.Message}."))
                .Execute(() => factory.CreateConnection(name));
        }
    }
}