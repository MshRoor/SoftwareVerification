
using Microsoft.Data.SqlClient;
using System.Data;

namespace DataAccess.DbAccess
{
    public interface ISqlDataAccess
    {
        IDbConnection CreateConnection(string _connectionString = "XcelDbConnection");
        IDbConnection CreateXcelHspConnection(string _connectionString = "XcelDbConnection");
        Task<IEnumerable<T>> LoadDataAsync<T, U>(string query, U parameters, string connectionId = "XcelDbConnection", CommandType cmdType = CommandType.Text);
        Task<int> SaveDataAsync<T>(string query, T parameters, string connectionId = "XcelDbConnection", CommandType cmdType = CommandType.Text);
        Task ExecuteSPAsync<T>(string storedProcedure, T parameters, string connectionId = "XcelDbConnection");
        //Task LoadSPDataAsync<T>(string storedProcedure, T parameters, string connectionId = "XcelDbConnection");
    }
}