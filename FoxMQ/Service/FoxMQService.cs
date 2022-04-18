using FoxMQ.Context;
using FoxMQ.Proto;
using Grpc.Core;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Google.Protobuf.WellKnownTypes;
using System.Globalization;
using FoxMQ.Model;

namespace FoxMQ.Service
{
    public class FoxMQService : FoxMQGrpcService.FoxMQGrpcServiceBase
    {
        private object _lock = new object();
        private MessageQueueData _queueData;

        public FoxMQService(ILoggerFactory logger, FoxMQContext context)
        {
            _queueData = new MessageQueueData();
        }

        public override async Task<PublishResponse> Publish(PublishRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Queue))
                throw new ArgumentNullException(nameof(request.Queue));

            if (string.IsNullOrWhiteSpace(request.Body))
                throw new ArgumentNullException(nameof(request.Body));
                        
            bool status = false;

            long id = await _queueData.Add(request.Queue, request.Body);

            if (id > 0)
            {
                status = true;
            }

            return await Task.FromResult(new PublishResponse { Id = id, Status = status });
        }

        public override async Task<GetResponse> Get(GetRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Queue))
                throw new ArgumentNullException(nameof(request.Queue));

            bool status = false;
            var message = new MessageQueue();

            lock (_lock)
            {
                message = _queueData.Get(request.Queue);

                if (message.Id > 0 && _queueData.Lock(message.Id) > 0)
                {
                    status = true;
                }
            }

            return await Task.FromResult(new GetResponse
            {
                Body = message.Body,
                Id = message.Id,
                Insertiondate = Timestamp.FromDateTimeOffset(message.InsertionDate),
                Status = status
            });
        }

        public override async Task<ConfirmResponse> Confirm(ConfirmRequest request, ServerCallContext context)
        {
            if (request.Id < 1)
                throw new ArgumentOutOfRangeException(nameof(request.Id));

            if (string.IsNullOrWhiteSpace(request.Queue))
                throw new ArgumentNullException(nameof(request.Queue));

            bool status = false;

            if (await _queueData.Remove(request.Id) > 0)
            {
                status = true;
            }

            return await Task.FromResult(new ConfirmResponse { Status = status });
        }

    }
}