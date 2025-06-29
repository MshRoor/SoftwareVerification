using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataAccess.DbAccess
{
    public class SqlDataAccess : ISqlDataAccess
    {
        private readonly IConfiguration _config;
        public SqlDataAccess(IConfiguration config)
        {
            _config = config;
        }
        public IDbConnection CreateConnection(string _connectionString = "XcelDbConnection") => new SqlConnection(_config.GetConnectionString(_connectionString));
        public IDbConnection CreateXcelHspConnection(string _connectionString = "XcelDbConnection") => new SqlConnection(_config.GetConnectionString(_connectionString));
        public async Task<IEnumerable<T>> LoadDataAsync<T, U>(string query, U parameters, string connectionId = "XcelDbConnection", CommandType cmdType = CommandType.Text)
        {
            using IDbConnection connection = CreateConnection(connectionId);
            return await connection.QueryAsync<T>(query, parameters, commandType: cmdType);
        }

        public async Task<int> SaveDataAsync<T>(string query, T parameters, string connectionId = "XcelDbConnection", CommandType cmdType = CommandType.Text)
        {
            using IDbConnection connection = CreateConnection(connectionId);
            return await connection.ExecuteAsync(query, parameters, commandType: cmdType);
        }
        public async Task ExecuteSPAsync<T>(string storedProcedure, T parameters, string connectionId = "XcelDbConnection")
        {
            using IDbConnection connection = CreateConnection(connectionId);
            await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        //public async Task LoadSPDataAsync<T>(string storedProcedure, T parameters, string connectionId = "XcelDbConnection")
        //{
        //    using IDbConnection connection = CreateConnection(connectionId);
        //    await connection.QueryFirstOrDefaultAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        //}

    }
}
