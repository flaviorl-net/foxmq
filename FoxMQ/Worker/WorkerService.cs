using FoxMQ.Context;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace FoxMQ.Worker
{
    public class WorkerService : BackgroundService
    {
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                new MessageQueueData().UnLock(1);

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
