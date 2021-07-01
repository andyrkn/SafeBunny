using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SafeBunny.Core.Connection;
using SafeBunny.Core.Extensions;
using SafeBunny.Core.Message;
using SafeBunny.Core.Pipeline;
using SafeBunny.Core.Serialization;
using SafeBunny.Core.Subscribing.Pipeline;
using SafeBunny.Core.Subscribing.Topology.Exceptions;
using SafeBunny.Core.Subscribing.Topology.Subscription;

namespace SafeBunny.Core.Subscribing.Topology
{
    internal sealed class TopologyConsumer : ITopologyConsumer
    {
        private readonly ITopology _topology;
        private readonly ISafeBunnySerializer _serializer;
        private readonly ISafeBunnyConnectionFactory _connectionFactory;
        private readonly IPipeline<IProcessingContext> _pipeline;
        private readonly ILogger<TopologyConsumer> _logger;
        private readonly SafeBunnySettings _settings;

        private readonly List<MaterializedSubscription> _subs = new ();
        private readonly IDictionary<string, AsyncEventingBasicConsumer> _consumers = new Dictionary<string, AsyncEventingBasicConsumer>();

        private IConnection _connection;

        public TopologyConsumer(
            ITopology topology,
            ISafeBunnyConnectionFactory connectionFactory,
            ISafeBunnySerializer serializer,
            IPipeline<IProcessingContext> pipeline,
            ILogger<TopologyConsumer> logger,
            IOptions<SafeBunnySettings> settings)
        {
            _topology = topology;
            _connectionFactory = connectionFactory;
            _serializer = serializer;
            _pipeline = pipeline;
            _logger = logger;
            _settings = settings.Value;
        }

        public void Start()
        {
            if (!_topology.Subscriptions().Any()) return;

            _connection = _connectionFactory.CreateConnection($"{AppDomain.CurrentDomain.FriendlyName}.Consumer");
            
            var exchangeChannel = _connection.CreateModel();
            exchangeChannel.ExchangeDeclare(TopologyExchanges.Delayed, ExchangeType.Direct,true,false, new Dictionary<string, object>
            {
                { "x-delayed-type", "direct"}
            });

            exchangeChannel.ExchangeDeclare(TopologyExchanges.Topic, ExchangeType.Topic, true, false);
            
            _topology.Subscriptions().ForEach(subscription =>
            {
                subscription.handlers.ForEach(handler 
                    => this.GetType().GetMethod(nameof(Subscribe), BindingFlags.NonPublic | BindingFlags.Instance)
                    .MakeGenericMethod(handler.MessageType).Invoke(this, new object[] {subscription.node, handler.QoS}));
            });

            exchangeChannel.Close();
            ResumeAll();
        }

        private void Subscribe<T>(string node, int qos)
        {
            var channel = _connection.CreateModel();
            channel.BasicQos(0, (ushort) qos, false);
            var consumer = new AsyncEventingBasicConsumer(channel);
            
            var queueName = $"{_settings.Node}.{typeof(T).Name}";
            channel.QueueDeclare(queueName, true, false, false);
            channel.QueueBind(queueName, TopologyExchanges.Topic, $"{node}.{typeof(T).Name}");

            consumer.Received += ConsumerOnReceivedAsync<T>;
            consumer.Unregistered += (_, @event) => Task.Run(() => @event.ConsumerTags.ToList().ForEach(tag => _consumers.Remove(tag)));
            channel.ModelShutdown += (_, args) => _logger.LogError($"Channel {typeof(T).Name} exception: {args.ReplyCode}-{args.ReplyText}-{args.Initiator}. Recovering...");
            _subs.Add(new(consumer, channel, queueName, typeof(T)));
        }

        private async Task ConsumerOnReceivedAsync<T>(object sender, BasicDeliverEventArgs @event)
        {
            _logger.LogInformation($"Message received {@event.DeliveryTag}");
            
            if (!_consumers.TryGetValue(@event.ConsumerTag, out var consumer))
            {
                throw new ConsumerException("No consumer registered for message");
            }

            var props = @event.BasicProperties.ToUserProps();
            var metadata = _topology.FirstOrDefault(typeof(T));
            
            object message = null;
            try
            {
                message = _serializer.Deserialize(@event.Body.ToArray(), metadata.MessageType);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}-{ex.StackTrace}");
                if (!@event.Redelivered)
                {
                    consumer.Model.BasicReject(@event.DeliveryTag, true);
                }
                return;
            }

            _logger.BeginScope($"{props.CorrelationId}|{props.MessageId}|");

            try
            {
                var context = metadata.ReflectionMetadata.NewContext(message, props, metadata);
                await _pipeline.ProcessAsync(context);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"{ex.Message}-{ex.StackTrace}");
            }
            finally
            {
                consumer.Model.BasicAck(@event.DeliveryTag, false);
            }
        }
        public void Resume<T>() => _subs
            .Where(sub => sub.MessageType == typeof(T))
            .ToList()
            .Execute(Resume);

        private void ResumeAll() => Resume(_subs);
        
        private void Resume(List<MaterializedSubscription> subs) => subs.ForEach(sub =>
        {
            var consumerTag = sub.Model.BasicConsume(sub.Queue, false, sub.Consumer);
            _consumers.Add(consumerTag, sub.Consumer);
            _logger.LogInformation($"CircuitBreaker: Consuming {sub.Queue}");
        });


        public void Cancel<T>() => _subs
            .Where(sub => sub.MessageType == typeof(T))
            .ToList()
            .Execute(Cancel);

        private void CancelAll() => Cancel(_subs);

        private void Cancel(List<MaterializedSubscription> subs) => subs.ForEach(sub =>
        {
            _logger.LogInformation(string.Join(",", sub.Consumer.ConsumerTags));
            sub.Model.BasicCancel(sub.Consumer.ConsumerTags.First());
            _logger.LogInformation($"CircuitBreaker: Disconnected from {sub.Queue}");
        });
    }
}