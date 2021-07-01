using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SafeBunny.Core.Connection;

namespace SafeBunny.Core.Publishing.Infrastructure
{
    internal sealed class PublishChannelsContainer : IPublishChannelsContainer
    {
        private readonly string _connectionName = $"{AppDomain.CurrentDomain.FriendlyName}.Publisher";
        private IConnection _connection;
        private readonly ISafeBunnyConnectionFactory _factory;
        private readonly ILogger<PublishChannelsContainer> _logger;
        private readonly SafeBunnySettings _settings;

        private int _bufferSize = 0;
        private int _availableBuffer = 0;
        private readonly BufferBlock<SafeChannel> _channels = new();

        public PublishChannelsContainer(
            ISafeBunnyConnectionFactory factory, 
            ILogger<PublishChannelsContainer> logger,
            IOptions<SafeBunnySettings> settings)
        {
            _factory = factory;
            _logger = logger;
            _settings = settings.Value;
            _connection = _factory.CreateConnection(_connectionName);
            _connection.ConnectionShutdown += OnConnectionShutdown;
        }

        public async Task<SafeChannel> Get()
        {
            if (_availableBuffer > 0 || _bufferSize >= _settings.MaximumPublishConcurrency)
            {
                var channel =  await _channels.ReceiveAsync().ConfigureAwait(false);
                Interlocked.Decrement(ref _availableBuffer);
                if (channel.Channel.IsClosed)
                {
                    channel.Channel.Dispose();
                    return new SafeChannel(_connection, _logger);
                }

                return channel;
            }

            Interlocked.Increment(ref _bufferSize);
            var newChannel = new SafeChannel(_connection, _logger);
            return newChannel;
        }

        public void Return(SafeChannel channel)
        {
            // _logger.LogInformation($"Channel returned, size: {_availableBuffer}-{_bufferSize}");
            if (channel.Channel.IsClosed)
            {
                channel.Dispose();
                _channels.Post(new SafeChannel(_connection, _logger));
            }
            else
            {
                _channels.Post(channel);
            }

            Interlocked.Increment(ref _availableBuffer);
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (e.Initiator == ShutdownInitiator.Application)
            {
                return;
            }

            _logger.LogError($"Connection to RabbitMQ lost:{e.ReplyText} {e.ReplyCode}, {e.Cause}");
            _connection = _factory.CreateConnection(_connectionName);
        }
    }
}