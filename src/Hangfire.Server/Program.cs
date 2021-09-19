using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Hangfire.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);

            var connectionString = builder.Build().GetConnectionString("HangfireDb");

            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);

            var hostBuilder = new HostBuilder()
                // Add configuration, logging, ...
                .ConfigureServices((hostContext, services) =>
                {
                    // Add your services for dependency injection
                });

            using(var server = new BackgroundJobServer(new BackgroundJobServerOptions()
			{
                WorkerCount = 1
			}))
			{
                await hostBuilder.RunConsoleAsync();
			}
        }
    }
}
