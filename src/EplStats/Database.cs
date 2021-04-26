using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace EplStats
{
    public interface IDatabase
    {
        Task<IEnumerable<T>> ExecuteQueryAsync<T>(string query);
    }

    public class Database : IDatabase
    {
        private const string EplStatsDb = "EplStatsDb";
        private readonly IConfiguration _configuration;
        public Database(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string query)
        {
            await using (var conn = new NpgsqlConnection(_configuration.GetConnectionString(EplStatsDb)))
            return conn.Query<T>(query);
        }

        
    }
}