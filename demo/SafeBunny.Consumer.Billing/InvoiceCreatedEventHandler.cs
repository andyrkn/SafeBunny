using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SafeBunny.Core.Message;
using SafeBunny.Core.Subscribing.Pipeline;
using SafeBunny.Domain;
using SafeBunny.SignalR;

namespace SafeBunny.Consumer.Billing
{
    public class InvoiceCreatedEventHandler : IMessageHandler<InvoiceCreatedEvent>
    {
        private readonly ISignalRClient client;
        private readonly ILogger<InvoiceCreatedEventHandler> _logger;

        public InvoiceCreatedEventHandler(ISignalRClient client, ILogger<InvoiceCreatedEventHandler> logger)
        {
            this.client = client;
            _logger = logger;
        }

        public async Task HandleAsync(IProcessingContext<InvoiceCreatedEvent> context)
        {
            await client.NotifyEvent($"Will attempt to bill order price {context.Message.Price}");
            await context.TransactAsync(() => client.NotifyTransaction($"Customer billed {context.Message.Price}"));
        }
    }
}