using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SafeBunny.Core.Extensions;
using SafeBunny.Core.Message;
using SafeBunny.Core.Publishing;

namespace SafeBunny.IntegrationBenchmarks.ConsumptionBenchmark
{
    public class SafeBunnyPublisher
    {
        public async Task Run()
        {
            var config = (IConfiguration)new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();
            var provider = new ServiceCollection()
                .AddLogging()
                .AddSafeBunny(config)
                .BuildServiceProvider()
                .UseSafeBunny();

            var publisher = provider.GetRequiredService<ISafeBunnyPublisher>();
            var props = new MessageProperties
            {
                Expiration = TimeSpan.FromHours(1)
            };
            var model = new SomePublishModel(1, "100");

            var x = new int[10000].Select(async _ => await publisher.PublishAsync(model, props));
            await Task.WhenAll(x);
        }
    }
}