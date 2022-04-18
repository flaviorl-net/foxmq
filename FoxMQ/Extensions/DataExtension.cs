using FoxMQ.Model;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace FoxMQ.Extensions
{
    public static class DataExtension
    {
        public static SqlCommand GetCommand(SqlConnection connection, string sql, SqlParameter[] parameters)
        {
            using SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.Clear();
            command.Parameters.AddRange(parameters);

            connection.Open();
            return command;
        }

        public static async Task<int> NonQueryAsync(this SqlConnection connection, string sql, SqlParameter[] parameters) => 
            await GetCommand(connection, sql, parameters)
            .ExecuteNonQueryAsync();

        public static int NonQuery(this SqlConnection connection, string sql, SqlParameter[] parameters) => 
            GetCommand(connection, sql, parameters)
            .ExecuteNonQuery();

        public static SqlDataReader Query(this SqlConnection connection, string sql, SqlParameter[] parameters) => 
            GetCommand(connection, sql, parameters)
            .ExecuteReader();
        
        public async static Task<SqlDataReader> QueryAsync(this SqlConnection connection, string sql, SqlParameter[] parameters) => 
            await GetCommand(connection, sql, parameters)
            .ExecuteReaderAsync();
    }
}