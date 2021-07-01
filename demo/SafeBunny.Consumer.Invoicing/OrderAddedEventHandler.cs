using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SafeBunny.Core.Message;
using SafeBunny.Core.Subscribing.Pipeline;
using SafeBunny.Domain;
using SafeBunny.SignalR;

namespace SafeBunny.Consumer.Invoicing
{
    internal sealed class OrderAddedEventHandler : IMessageHandler<OrderAddedEvent>
    {
        private readonly ISignalRClient client;
        private readonly ILogger<OrderAddedEventHandler> _logger;

        public OrderAddedEventHandler(ISignalRClient client, ILogger<OrderAddedEventHandler> logger)
        {
            this.client = client;
            _logger = logger;
        }

        public async Task HandleAsync(IProcessingContext<OrderAddedEvent> context)
        {
            var price = DateTime.Now.Millisecond % 256;
            await client.NotifyEvent($"Will attempt to set order price {price}");
            await context.TransactAsync(() => client.NotifyTransaction($"Setting price {price}"));
            await context.ContinueAsync(new InvoiceCreatedEvent(price));
            
            _logger.LogInformation($"marker: {context.Properties.ContinuationMarker}");

            if (DateTime.Now.Millisecond % 10 > 5)
            {
                await client.NotifyException($"Exception occurred {context.Message.Name} with price {price}");
                throw new Exception();
            }
        }
    }
}