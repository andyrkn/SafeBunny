using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using MassTransit;
using SafeBunny.IntegrationBenchmarks.ConsumptionBenchmark;

namespace SafeBunny.IntegrationBenchmarks
{
    public static class Program
    {
        private static void RunBenchmarks<T>()
        {
            BenchmarkRunner.Run<T>(
                DefaultConfig.Instance
                    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core50)
                        .WithWarmupCount(8)
                        .WithInvocationCount(128)
                        .WithIterationCount(16))
                    .AddDiagnoser(MemoryDiagnoser.Default)
                    .AddExporter(CsvMeasurementsExporter.Default)
                    .AddExporter(RPlotExporter.Default)
                    );
        }

        public static async Task Main(string[] args)
        {
            //var massTransit = new MassTransitConsumer();
            //var massTransit = new MassTransitPublisher();
            //await massTransit.Run();

            var safeBunny = new SafeBunnyPublisher();
            //var safeBunny = new SafeBunnyConsumer();
            await safeBunny.Run();
        }
    }
}
