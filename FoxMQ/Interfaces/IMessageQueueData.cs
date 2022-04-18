using FoxMQ.Model;
using System.Threading.Tasks;

namespace FoxMQ.Interfaces
{
    public interface IMessageQueueData
    {
        Task<long> Add(string queueName, string body);
        MessageQueue Get(string queueName);
        int Lock(long id);
        Task<int> Remove(long id);
        Task<int> UnLock(int minutesWait);
    }
}