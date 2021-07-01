using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SafeBunny.Core.Extensions;

namespace SafeBunny.IntegrationBenchmarks.ConsumptionBenchmark
{
    public class SafeBunnyConsumer
    {
        public async Task Run()
        {
            var config = (IConfiguration)new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();
            var provider = new ServiceCollection()
                .AddLogging()
                .AddSafeBunny(config)
                .AddSafeBunnyMessageHandlers(typeof(SafeBunnyConsumer).Assembly)
                .BuildServiceProvider()
                .UseSafeBunnySubscriptions(x =>
                {
                    x
                        .FromNode("sample-consumer")
                        .Consume<SomePublishModel>(10);
                })
                .UseSafeBunny();


            Console.ReadKey();
            await LogHeaven.Write();
        }
    }
}