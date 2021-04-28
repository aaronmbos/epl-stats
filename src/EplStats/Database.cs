using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace EplStats
{
    public interface IDatabase
    {
        Task<IEnumerable<T>> ExecuteQueryAsync<T>(string querySql);
        Task ExecuteCommandAsync(string commandSql, object parameters);
    }

    public class Database : IDatabase
    {
        private const string EplStatsDb = "EplStatsDb";
        private readonly IConfiguration _configuration;
        public Database(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string querySql)
        {
            await using (var conn = new NpgsqlConnection(_configuration.GetConnectionString(EplStatsDb)))
            return conn.Query<T>(querySql);
        }

        public async Task ExecuteCommandAsync(string commandSql, object parameters)
        {
            await using (var conn = new NpgsqlConnection(_configuration.GetConnectionString(EplStatsDb)))
            conn.Execute(commandSql, parameters);
        }
    }
}