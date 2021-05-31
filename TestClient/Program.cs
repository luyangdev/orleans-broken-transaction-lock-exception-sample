using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
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
            using var client = BuildClientSqlServer();

            await client.Connect();

            var tasks = Enumerable.Range(1, 3).Select(async _ => await client.GetGrain<ITaskGrain>("key").Execute()).ToList();
            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
            {
                System.Console.WriteLine(result.Count);
            }
        }

        private static IClusterClient BuildClientSqlServer()
        {
            return new ClientBuilder()
                .UseAdoNetClustering(options =>
                {
                    options.Invariant = Constants.SqlServerInvariant;
                    options.ConnectionString = Constants.SqlServerConnectionString;
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ServiceId = Constants.ServiceId;
                    options.ClusterId = Constants.ClusterId;
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();
        }

        private static IClusterClient BuildClientInMemory()
        {
            return new ClientBuilder()
                .UseLocalhostClustering()
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();
        }
    }
}