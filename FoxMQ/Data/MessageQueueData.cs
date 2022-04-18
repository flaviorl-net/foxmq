using FoxMQ.Extensions;
using FoxMQ.Interfaces;
using FoxMQ.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace FoxMQ
{
    public class MessageQueueData : IMessageQueueData
    {
        private string _strConnection = "Server=.\\SQLEXPRESS;Database=FoxMQ;Trusted_Connection=True;MultipleActiveResultSets=True";

        public async Task<long> Add(string queueName, string body)
        {
            try
            {
                var parameters = new SqlParameter[4];
                parameters[0] = new SqlParameter("insertionDate", SqlDbType.DateTime) { Value = DateTime.Now };
                parameters[1] = new SqlParameter("lock", SqlDbType.Bit) { Value = false };
                parameters[2] = new SqlParameter("queueName", SqlDbType.VarChar, 100) { Value = queueName };
                parameters[3] = new SqlParameter("body", SqlDbType.VarChar, -1) { Value = body };

                using SqlConnection connection = new SqlConnection(_strConnection);
                SqlDataReader reader = await connection.QueryAsync(" insert into MessageQueue (QueueName, InsertionDate, Lock, Body) values (@queueName, @insertionDate, @lock, @body); " +
                             " select CAST(SCOPE_IDENTITY() as bigint) ", parameters);

                if (reader.HasRows && reader.Read() && !reader.IsDBNull(0))
                {
                    return reader.GetInt64(0);
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public MessageQueue Get(string queueName)
        {
            try
            {
                var parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("queueName", SqlDbType.VarChar, 100) { Value = queueName };

                using SqlConnection connection = new SqlConnection(_strConnection);
                SqlDataReader reader = connection.Query(" select Id, InsertionDate, Lock, QueueName, Body " +
                             " from MessageQueue with(nolock) " +
                             " where Lock = 0 " +
                             " and QueueName = @queueName " +
                             " and id = (select min(id) from MessageQueue with(nolock) where Lock = 0 and QueueName = @queueName) ", parameters);

                if (reader.HasRows && reader.Read() && !reader.IsDBNull(0))
                {
                    return new MessageQueue()
                    {
                        Id = reader.GetInt64(0),
                        InsertionDate = reader.GetDateTime(1),
                        Lock = reader.GetBoolean(2),
                        QueueName = reader.GetString(3),
                        Body = reader.GetString(4),
                    };
                }

                return new MessageQueue();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public int Lock(long id)
        {
            try
            {
                var parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("id", SqlDbType.BigInt) { Value = id };

                using SqlConnection connection = new SqlConnection(_strConnection);
                return connection.NonQuery("update MessageQueue set Lock = 1 where Id = @id", parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> UnLock(int minutesWait)
        {
            try
            {
                var parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("minutesWait", SqlDbType.Int) { Value = minutesWait };

                using SqlConnection connection = new SqlConnection(_strConnection);
                return await connection.NonQueryAsync("update MessageQueue set Lock = 0 " +
                             " where Id in ( " +
                             "    select top 100 Id from MessageQueue with(nolock) " +
                             "    where Lock = 1 " +
                             "    and dateadd(minute, @minutesWait, InsertionDate) < getdate() " +
                             " )", parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> Remove(long id, string queueName)
        {
            try
            {
                var parameters = new SqlParameter[2];
                parameters[0] = new SqlParameter("id", SqlDbType.BigInt) { Value = id };
                parameters[1] = new SqlParameter("queueName", SqlDbType.BigInt) { Value = queueName };

                using SqlConnection connection = new SqlConnection(_strConnection);
                return await connection.NonQueryAsync("delete MessageQueue where Id = @id and QueueName = @queueName", parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}