using System.Threading.Tasks;

namespace EplStats
{
    public interface IApp
    {
        Task RunAsync();
    }
    
    public class App : IApp
    {
        private readonly IScript _script;

        public App(IScript script) => _script = script;

        public async Task RunAsync()
        {
            var teamsTask = Task.FromResult<object>(null);//_script.UpsertTeams();
            var playersTask = _script.UpsertPlayers();
            
            await Task.WhenAll(teamsTask, playersTask);
        }
    }
}
