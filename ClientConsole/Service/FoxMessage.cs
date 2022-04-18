using FoxMQ.Proto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClientConsole.Service
{
    public class FoxMessage : IFoxMessage
    {
        private readonly  FoxMQGrpcService.FoxMQGrpcServiceClient _client;

        public FoxMessage(FoxMQGrpcService.FoxMQGrpcServiceClient queue)
        {
            _client = queue;
        }

        public async Task<PublishResponse> PublishMessage()
        {
            try
            {
                var response = await _client.PublishAsync(new PublishRequest()
                {
                    Queue = "fila 1",
                    Body = "{\"nome\": \"flavio\"}"
                });
                return response;
            }
            catch (Grpc.Core.RpcException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }
    }

    public interface IFoxMessage
    {
        Task<PublishResponse> PublishMessage();
    }
}
