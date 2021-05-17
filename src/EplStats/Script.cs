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
            if (newTeams.Any())
            {
                await _database.ExecuteCommandAsync(SqlStatements.InsertTeamsSql, newTeams.Select(x => new { Name = x, IsActive = true }));
            }

            var inactiveTeams = currentTeams.Where(x => x.IsActive).Select(x => x.Name).Except(scrapedTeams);
            if (inactiveTeams.Any())
            {
                await _database.ExecuteCommandAsync(string.Format(SqlStatements.UpdateTeamsSql, "False"), inactiveTeams.Select(x => new { Name = x }));
            }

            var activeTeams = currentTeams.Where(x => !x.IsActive).Select(x => x.Name).Intersect(scrapedTeams);
            if (activeTeams.Any())
            {
                await _database.ExecuteCommandAsync(string.Format(SqlStatements.UpdateTeamsSql, "True"), activeTeams.Select(x => new { Name = x }));
            }
        }

        public static class SqlStatements
        {
            public static string GetAllTeamsSql => "SELECT team_id AS Id, team_name AS Name, is_active AS IsActive FROM epl.teams";
            public static string InsertTeamsSql => "INSERT INTO epl.teams (team_name, is_active) VALUES (@Name, @IsActive)";
            public static string UpdateTeamsSql => "UPDATE epl.teams SET is_active = {0} WHERE team_name = @Name";
        }
    }
}