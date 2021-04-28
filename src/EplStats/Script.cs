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
            var currentTeams = await _database.ExecuteQueryAsync<Team>(SqlStatements.GetAllTeamsSql);
            var scrapedTeams = _scraper.ScrapeTeams();

            var newTeams = scrapedTeams.Except(currentTeams.Select(x => x.Name));
            // Not efficent for bulk updates, but should only need to insert teams once a season
            await _database.ExecuteCommandAsync(SqlStatements.InsertTeamsSql, newTeams.Select(x => new {Name = x, IsActive = true}));

            var updatedTeams = currentTeams.Select(x => x.Name).Except(scrapedTeams);
        }

        public static class SqlStatements
        {
            public static string GetAllTeamsSql => "select team_id as Id, team_name as Name, is_active as IsActive from epl.teams";
            public static string InsertTeamsSql => "insert into epl.teams (team_name, is_active) values (@Name, @IsActive)";
        }
    }
}