using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using Dapper;
using Npgsql;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace EplStats
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            
            var configuration = host.Services.GetRequiredService<IConfiguration>();
            var connStr = configuration.GetConnectionString("EplStatsDb");
            await new App(configuration).Run();
            // await using (var conn = new NpgsqlConnection(connStr))
            // {
            //     await conn.OpenAsync();
            //     var test = conn.Query("select * from epl.teams;");
            //     Console.WriteLine(test.AsList().Count);
            // }
            // using (IWebDriver driver = new FirefoxDriver())
            // {


            //     driver.Navigate().GoToUrl(@"https://fantasy.premierleague.com/statistics");
            //     var element = driver.FindElement(By.CssSelector("#filter>optgroup[label=\"By Team\"]>option:nth-child(1)"));
            //     element.Click();
            //     var players = driver.FindElements(By.CssSelector("table tbody tr"));
            //     //var teams = element.Text.Split(Environment.NewLine);
            //     Console.WriteLine(element.Text);
            //     driver.Quit();
            // }
        }
        static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) => 
            {
                
            });
    }
}
