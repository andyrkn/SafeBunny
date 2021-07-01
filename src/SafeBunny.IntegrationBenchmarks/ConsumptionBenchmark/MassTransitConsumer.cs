using System;
using System.Threading.Tasks;
using MassTransit;

namespace SafeBunny.IntegrationBenchmarks.ConsumptionBenchmark
{
    public class MassTransitConsumer
    {
        public async Task Run()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(c =>
            {
                c.Durable = true;
                c.AutoStart = true;
                c.DeployTopologyOnly = false;
                c.Host("localhost", "/", c =>
                {
                    c.PublisherConfirmation = true;
                    c.Username("ms-1");
                    c.Password("ms-1");
                });
            }); 
            busControl.ConnectReceiveEndpoint("masstransit-endpoint", configurator =>
            {
                configurator.AutoStart = true;
                configurator.PrefetchCount = 10;
                configurator.ConcurrentMessageLimit = 10;
                configurator.Consumer<MassTransitBenchmarkConsumer>();
            });
            busControl.Start();


            Console.ReadKey();
            await LogHeaven.Write();
        }
    }
}