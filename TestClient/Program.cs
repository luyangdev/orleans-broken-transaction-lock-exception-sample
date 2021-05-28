using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestGrains;

namespace TestClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            using var client = new ClientBuilder()
                .UseLocalhostClustering()
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();

            var tasks = Enumerable.Range(1, 100).Select(async _ => await client.GetGrain<ITaskGrain>("key").Execute()).ToList();
            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
            {
                System.Console.WriteLine(result.Count);
            }
        }
    }
}