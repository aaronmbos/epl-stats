using System.Linq;
using System.Threading.Tasks;

namespace EplStats
{
    public interface IScript
    {
        Task UpsertTeams();
    }

    public class Script : IScript
    {
        private readonly IDatabase _database;
        private readonly IScraper _scraper;

        public Script(IDatabase database, IScraper scraper)
        {
            _database = database;
            _scraper = scraper;
        }

        public async Task UpsertTeams()
        {
            var currentTeams = await _database.ExecuteQueryAsync<Team>("select team_id as Id, team_name as Name, is_active as IsActive from epl.teams");
            var scrapedTeams = _scraper.ScrapeTeams();

            var newTeams = scrapedTeams.Except(currentTeams.Select(x => x.Name));
            var updateTeams = currentTeams.Select(x => x.Name).Except(scrapedTeams);
        }
    }
}