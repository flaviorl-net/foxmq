using FoxMQ.Model;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace FoxMQ.Extensions
{
    public static class DataExtension
    {
        public static async Task<int> NonQueryAsync(this SqlConnection connection, string sql, SqlParameter[] parameters)
        {
            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.Clear();
            command.Parameters.AddRange(parameters);

            connection.Open();
            return await command.ExecuteNonQueryAsync();
        }

        public static int NonQuery(this SqlConnection connection, string sql, SqlParameter[] parameters)
        {
            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.Clear();
            command.Parameters.AddRange(parameters);

            connection.Open();
            return command.ExecuteNonQuery();
        }

        public static SqlDataReader Query(this SqlConnection connection, string sql, SqlParameter[] parameters)
        {
            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.Clear();
            command.Parameters.AddRange(parameters);

            connection.Open();
            return command.ExecuteReader();
        }

        public async static Task<SqlDataReader> QueryAsync(this SqlConnection connection, string sql, SqlParameter[] parameters)
        {
            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.Clear();
            command.Parameters.AddRange(parameters);

            connection.Open();
            return await command.ExecuteReaderAsync();
        }

        public static MessageQueue QueryMessageQueue(this SqlConnection connection, string sql, SqlParameter[] parameters)
        {
            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.Clear();
            command.Parameters.AddRange(parameters);

            connection.Open();
            SqlDataReader dataReader = command.ExecuteReader();

            if (dataReader.HasRows && dataReader.Read() && !dataReader.IsDBNull(0))
            {
                return new MessageQueue()
                {
                    Id = dataReader.GetInt64(0),
                    InsertionDate = dataReader.GetDateTime(1),
                    Lock = dataReader.GetBoolean(2),
                    QueueName = dataReader.GetString(3),
                    Body = dataReader.GetString(4),
                };
            }

            return new MessageQueue();
        }
    }
}