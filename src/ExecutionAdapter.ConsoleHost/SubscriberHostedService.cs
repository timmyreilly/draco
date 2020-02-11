using Draco.Core.Execution.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Draco.ExecutionAdapter.ConsoleHost
{
    public class SubscriberHostedService : IHostedService
    {
        private readonly IExecutionRequestSubscriber requestSubscriber;
        private readonly ILogger logger;

        public SubscriberHostedService(
            IExecutionRequestSubscriber requestSubscriber, 
            ILogger<SubscriberHostedService> logger)
        {
            this.requestSubscriber = requestSubscriber;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Subscribing to incoming execution requests...");

            await this.requestSubscriber.SubscribeAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
