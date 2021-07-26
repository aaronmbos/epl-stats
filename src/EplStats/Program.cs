using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace EplStats
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            await host.Services.GetRequiredService<IApp>().RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) => 
            {
                services.AddTransient<IScript, Script>();
                services.AddTransient<IApp, App>();
                services.AddTransient<IDatabase, Database>();
                services.AddTransient<IScraper, Scraper>();
            });
    }
}
