using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SafeBunny.Core.Extensions;
using SafeBunny.CosmosDbStore;
using SafeBunny.SignalR;

namespace SafeBunny.Hosts
{
    public static class SafeBunnyHostsFactory
    {
        public static IHost GetHost(Assembly assembly) 
            => new HostBuilder()
                .ConfigureHostConfiguration(c => c.AddJsonFile("appsettings.json"))
                .ConfigureServices((c, s) => s
                    .AddSignalRClient(c.Configuration)
                    .AddSafeBunny(c.Configuration)
                    .AddSafeBunnyMessageHandlers(assembly)
                    .AddSafeBunnyCosmosStore(c.Configuration))
                .ConfigureLogging((c, l) => l
                    .ClearProviders()
                    .AddConsole()
                    .AddConfiguration(c.Configuration.GetSection("Logging")))
                .UseConsoleLifetime()
                .Build();
    }
}
