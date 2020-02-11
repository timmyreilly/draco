using Draco.Core.Execution.Interfaces;
using Draco.Core.Execution.Options;
using Draco.Core.Interfaces;
using Draco.Core.Models;
using Draco.Core.Models.Enumerations;
using Draco.Core.Models.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Draco.Core.Execution.Processors
{
    public class AsyncExecutionProcessor<TAdapter> : IExecutionProcessor<TAdapter> 
        where TAdapter : IExecutionAdapter
    {
        private readonly IExecutionAdapter execAdapter;
        private readonly IJsonHttpClient jsonHttpClient;
        private readonly ILogger logger;
        private readonly IExecutionProcessorOptions processorOptions;

        public AsyncExecutionProcessor(
            TAdapter execAdapter,
            IJsonHttpClient jsonHttpClient,
            ILogger<AsyncExecutionProcessor<TAdapter>> logger,
            IOptionsSnapshot<ExecutionProcessorOptions<AsyncExecutionProcessor<TAdapter>>> processorOptionsSnapshot)
            : this(execAdapter, jsonHttpClient, logger, processorOptionsSnapshot.Value) { }

        public AsyncExecutionProcessor(
            IExecutionAdapter execAdapter,
            IJsonHttpClient jsonHttpClient,
            ILogger logger,
            IExecutionProcessorOptions processorOptions)
        {
            this.execAdapter = execAdapter;
            this.jsonHttpClient = jsonHttpClient;
            this.logger = logger;
            this.processorOptions = processorOptions;
        }

        public async Task<Core.Models.ExecutionContext> ProcessRequestAsync(ExecutionRequest execRequest, CancellationToken cancelToken)
        {
            if (execRequest == null)
            {
                throw new ArgumentNullException(nameof(execRequest));
            }

            var execContext = execRequest.ToExecutionContext().UpdateStatus(ExecutionStatus.Processing);

            logger.LogInformation($"Updating execution [{execRequest.ExecutionId}] status: [{execContext.Status}]...");

            await UpdateExecutionStatusAsync(execRequest.UpdateExecutionStatusUrl, execContext.ToExecutionUpdate());

            logger.LogInformation($"Processing execution request [{execRequest.ExecutionId}]...");

            execContext = await this.execAdapter.ExecuteAsync(execRequest, cancelToken);

            logger.LogInformation($"Updating execution [{execRequest.ExecutionId}] status: [{execContext.Status}]...");

            await UpdateExecutionStatusAsync(execRequest.UpdateExecutionStatusUrl, execContext.ToExecutionUpdate());

            return execContext;
        }

        private async Task UpdateExecutionStatusAsync(string updateUrl, ExecutionUpdate execUpdate) => 
            await jsonHttpClient.PutAsync(updateUrl, execUpdate);
    }
}
