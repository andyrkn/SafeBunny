using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace SafeBunny.Client.Hubs
{
    public class SafeBunnyHub : Hub
    {
        private ILogger<SafeBunnyHub> _logger;

        public SafeBunnyHub(ILogger<SafeBunnyHub> logger)
        {
            _logger = logger;
        }

        public void Subscribe(string node) { }
                    
        public async Task NotifyTransaction(string node, string data)
        {
            _logger.LogInformation($"{node}-{data}");
            await Clients.All.SendAsync("event", new EventModel("transaction", node, data));
        }

        public async Task NotifyEvent(string node, string data)
        {
            _logger.LogInformation($"{node}-{data}");
            await Clients.All.SendAsync("event", new EventModel("info", node, data));
        }

        public async Task NotifyException(string node, string data)
        {
            _logger.LogInformation($"{node}-{data}");
            await Clients.All.SendAsync("event", new EventModel("exception", node, data));
        }
    }
}