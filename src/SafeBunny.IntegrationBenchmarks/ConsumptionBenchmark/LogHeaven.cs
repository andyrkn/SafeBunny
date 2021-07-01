using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SafeBunny.IntegrationBenchmarks.ConsumptionBenchmark
{
    public static class LogHeaven
    {
        private const string path = @"logs.txt";

        public static BufferBlock<string> logs = new();

        public static void Add(string log)
        {
            logs.Post(log);
        }

        public static async Task Write()
        {
            var output = logs.TryReceiveAll(out var items);
            await File.WriteAllLinesAsync(path, items);
        } 
    }
}