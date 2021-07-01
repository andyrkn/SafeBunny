using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SafeBunny.Core.Subscribing.Topology.Subscription
{
    internal sealed class MaterializedSubscription
    {
        public readonly AsyncEventingBasicConsumer Consumer;
        public readonly IModel Model;
        public readonly string Queue;
        public readonly Type MessageType;

        public MaterializedSubscription(AsyncEventingBasicConsumer consumer, IModel model, string queue, Type messageType)
        {
            Consumer = consumer;
            Model = model;
            Queue = queue;
            MessageType = messageType;
        }
    }
}