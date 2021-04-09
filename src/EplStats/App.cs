using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace EplStats
{
    public class App
    {
        private readonly IConfiguration _configuration;

        public App(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Run()
        {
            
        }
    }
}