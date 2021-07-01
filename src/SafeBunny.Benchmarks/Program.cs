using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using SafeBunny.Benchmarks.Pipeline;
using SafeBunny.Benchmarks.Reflection;
using SafeBunny.Benchmarks.Serializers;

namespace SafeBunny.Benchmarks
{
    public static class Program
    {
        private static void RunBenchmarks<T>()
        {
            BenchmarkRunner.Run<T>(
                DefaultConfig.Instance
                    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core50))
                    .AddDiagnoser(MemoryDiagnoser.Default)
                    .AddExporter(CsvMeasurementsExporter.Default)
                    .AddExporter(RPlotExporter.Default)
                    );
        }
        public static void Main(string[] args)
        {
            // BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());
            switch (args[0])
            {
                case "pipeline": 
                    RunBenchmarks<PipelineBenchmarks>();
                    break;
                case "reflection":
                    RunBenchmarks<ReflectionBenchmarks>();
                    break;
                case "serialization":
                    RunBenchmarks<SerializerBenchmarks>();
                    break;
            }
        }
    }
}
