using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace EplStats
{
    public interface IApp
    {
        Task RunAsync();
    }

    public class App : IApp
    {
        private readonly IScript _script;

        public App(IScript script)
        {
            _script = script;
        }

        public async Task RunAsync()
        {
            await _script.UpsertTeams();
        }
    }
}