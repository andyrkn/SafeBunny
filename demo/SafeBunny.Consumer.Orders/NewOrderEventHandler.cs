using System;
using System.Threading.Tasks;
using SafeBunny.Core.Message;
using SafeBunny.Core.Subscribing.Pipeline;
using SafeBunny.Domain;
using SafeBunny.SignalR;

namespace SafeBunny.Consumer.Orders
{
    public class NewOrderEventHandler : IMessageHandler<NewOrderModel>
    {
        private readonly ISignalRClient client;

        public NewOrderEventHandler(ISignalRClient client)
        {
            this.client = client;
        }

        public async Task HandleAsync(IProcessingContext<NewOrderModel> context)
        {
            await client.NotifyEvent($"Attempting to save order {context.Message.Name}");
            await context.TransactAsync(async () =>
            {
                await client.NotifyTransaction($"Order saved {context.Message.Name}");
            });

            await context.ContinueAsync(new OrderAddedEvent(context.Message.Name));

            if (DateTime.Now.Millisecond % 10 > 5)
            {
                await client.NotifyException($"Exception occurred {context.Message.Name}");
                throw new Exception();
            }
        }
    }
}