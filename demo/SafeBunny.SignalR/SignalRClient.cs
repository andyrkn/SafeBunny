using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using SafeBunny.SignalR.HubResources;

namespace SafeBunny.SignalR
{
    internal sealed class SignalRClient : ISignalRClient
    {
        private readonly HubConnection connection;
        private readonly SignalRConfig config;

        public SignalRClient(IOptions<SignalRConfig> config)
        {
            this.config = config.Value;
            connection = new HubConnectionBuilder()
                .WithUrl(this.config.Host, opts =>
                {
                    opts.HttpMessageHandlerFactory = (message) => new HttpClientHandler
                    {
                        CheckCertificateRevocationList = false,

                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                })
                .WithAutomaticReconnect()
                .Build();
            connection.StartAsync().GetAwaiter().GetResult();
        }

        public async Task Subscribe() 
            => await connection.InvokeCoreAsync(SignalRResources.Subscribe, new object[] {config.Identity });

        public async Task NotifyTransaction(string data)
            => await connection.InvokeCoreAsync(SignalRResources.NotifyTransaction, new object[] { config.Identity, data });

        public async Task NotifyEvent(string data)
            => await connection.InvokeCoreAsync(SignalRResources.NotifyEvent, new object[] { config.Identity, data });

        public async Task NotifyException(string data)
            => await connection.InvokeCoreAsync(SignalRResources.NotifyException, new object[] { config.Identity, data });
    }
}