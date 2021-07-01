using System;
using System.Threading.Tasks;
using MassTransit;

namespace SafeBunny.IntegrationBenchmarks.ConsumptionBenchmark
{
    class MassTransitBenchmarkConsumer : IConsumer<SomePublishModel>
    {

        public Task Consume(ConsumeContext<SomePublishModel> context)
        {
            LogHeaven.Add(DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
            return Task.CompletedTask;
        }
    };
}