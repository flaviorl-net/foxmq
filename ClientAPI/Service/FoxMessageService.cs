using FoxMQ.Proto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClientAPI.Service
{
    public class FoxMessageService : IFoxMessage
    {
        private readonly FoxMQGrpcService.FoxMQGrpcServiceClient _client;

        public FoxMessageService(FoxMQGrpcService.FoxMQGrpcServiceClient queue)
        {
            _client = queue;
        }

        public async Task<GetResponse> Get(string queueName)
        {
            return await _client.GetAsync(new GetRequest()
            {
                Queue = queueName
            });
        }

        public async Task<PublishResponse> Publish(string queueName, string body)
        {
            try
            {
                return await _client.PublishAsync(new PublishRequest()
                {
                    Queue = queueName,
                    Body = body
                });
            }
            catch (Grpc.Core.RpcException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ConfirmResponse> Confirm(long id, string queueName)
        {
            return await _client.ConfirmAsync(new ConfirmRequest()
            {
                Id = id,
                Queue = queueName
            });
        }
    }

    public interface IFoxMessage
    {
        Task<PublishResponse> Publish(string queueName, string body);

        Task<GetResponse> Get(string queueName);

        Task<ConfirmResponse> Confirm(long id, string queueName);
    }
}
