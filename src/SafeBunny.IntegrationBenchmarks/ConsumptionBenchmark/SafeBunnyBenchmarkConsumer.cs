using System;
using System.Threading.Tasks;
using SafeBunny.Core.Message;
using SafeBunny.Core.Subscribing.Pipeline;

namespace SafeBunny.IntegrationBenchmarks.ConsumptionBenchmark
{
    public class SafeBunnyBenchmarkConsumer : IMessageHandler<SomePublishModel>
    {
        public Task HandleAsync(IProcessingContext<SomePublishModel> context)
        {
            LogHeaven.Add(DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
            return Task.CompletedTask;
        }
    }
}