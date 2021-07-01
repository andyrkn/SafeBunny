using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SafeBunny.Core.Extensions;
using SafeBunny.Core.Message;
using SafeBunny.Core.Publishing;

namespace SafeBunny.IntegrationBenchmarks
{
    public class PublishBenchmarks
    {
        //SB
        private IServiceProvider _provider { get; set; }
        private ISafeBunnyPublisher _safeBunnyPublisher { get; set; }

        //MT
        private IBusControl MassTransitBusControl { get; set; }

        // data
        private SomePublishModel model = new SomePublishModel(1, "SomePublishModel");
        private readonly MessageProperties props = new MessageProperties
        {
            Expiration = TimeSpan.FromMinutes(2)
        };

        public PublishBenchmarks()
        {
            ConfigureSafeBunny();
            // ConfigureMassTransit();
        }

        private void ConfigureSafeBunny()
        {
            var config = (IConfiguration) new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();
            _provider = new ServiceCollection()
                .AddLogging()
                .AddSafeBunny(config)
                .BuildServiceProvider()
                .UseSafeBunny();

            _safeBunnyPublisher = _provider.GetRequiredService<ISafeBunnyPublisher>();
        }

        private void ConfigureMassTransit()
        {
            MassTransitBusControl = Bus.Factory.CreateUsingRabbitMq(c =>
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
            MassTransitBusControl.Start();
            /*busControl.ConnectReceiveEndpoint("roxanica", c =>
            {
                c.Consumer<RoxanicaConsumer>();
            });*/
        }

        [Benchmark]
        public async Task SafeBunny_Publish()
        {
            await _safeBunnyPublisher.PublishAsync(model, props);
        }

        //[Benchmark]
        public async Task MassTransit_Publish()
        {
            await MassTransitBusControl.Publish(model, c =>
            {
                c.Durable = true;
            });
        }
    }
}