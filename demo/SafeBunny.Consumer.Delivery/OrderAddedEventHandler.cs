using System;
using System.Threading.Tasks;
using SafeBunny.Core.Message;
using SafeBunny.Domain;
using SafeBunny.Core.Subscribing.Pipeline;
using SafeBunny.SignalR;

namespace SafeBunny.Consumer.Delivery
{
    public class OrderAddedEventHandler : IMessageHandler<OrderAddedEvent>
    {
        private readonly ISignalRClient client;

        public OrderAddedEventHandler(ISignalRClient client)
        {
            this.client = client;
        }

        public async Task HandleAsync(IProcessingContext<OrderAddedEvent> context)
        {
            await client.NotifyEvent($"Will attempt to send to {context.Message.Name}");
            await context.TransactAsync(() => client.NotifyTransaction($"Delivery sent to {context.Message.Name}"));
        }
    }
}