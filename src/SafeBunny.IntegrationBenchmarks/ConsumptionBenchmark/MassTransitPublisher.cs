using System.Linq;
using System.Threading.Tasks;
using MassTransit;

namespace SafeBunny.IntegrationBenchmarks.ConsumptionBenchmark
{
    public class MassTransitPublisher
    {
        public async Task Run()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(c =>
            {
                c.Durable = true;
                c.AutoStart = true;
                c.DeployTopologyOnly = false;
                c.PrefetchCount = 10;
                c.Host("localhost", "/", c =>
                {
                    c.PublisherConfirmation = true;
                    c.Username("ms-1");
                    c.Password("ms-1");
                });
            });
            bus.Start();

            var x = new int[10000].Select(async _ => await bus.Publish(new SomePublishModel(1, "100")));
            await Task.WhenAll(x);
        }
    }
}