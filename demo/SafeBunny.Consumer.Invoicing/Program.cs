using System;
using System.Threading.Tasks;
using SafeBunny.Core.Extensions;
using SafeBunny.Core.Subscribing.Topology.Subscription.Retry;
using SafeBunny.Domain;
using SafeBunny.Hosts;

namespace SafeBunny.Consumer.Invoicing
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = SafeBunnyHostsFactory.GetHost(typeof(Program).Assembly);
            await host.StartAsync();

            host.Services.UseSafeBunnySubscriptions(subs =>
                subs.FromNode("orders")
                    .Consume<OrderAddedEvent>(1, RetryStrategy.From(3, TimeSpan.FromSeconds(1))));
            host.Services.UseSafeBunny();

            var tcs = new TaskCompletionSource();

            Console.CancelKeyPress += (sender, eventArgs) => tcs.SetResult();

            await tcs.Task;
            await host.StopAsync();
        }
    }
}
