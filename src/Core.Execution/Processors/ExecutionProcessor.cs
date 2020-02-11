using Draco.Core.Execution.Interfaces;
using Draco.Core.Execution.Options;
using Draco.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Draco.Core.Execution.Processors
{
    public class ExecutionProcessor<TAdapter> : IExecutionProcessor<TAdapter> 
        where TAdapter : IExecutionAdapter
    {
        private readonly IExecutionAdapter execAdapter;
        private readonly ILogger logger;
        private readonly IExecutionProcessorOptions processorOptions;

        public ExecutionProcessor(
            TAdapter execAdapter,
            ILogger<ExecutionProcessor<TAdapter>> logger,
            IOptionsSnapshot<ExecutionProcessorOptions<ExecutionProcessor<TAdapter>>> processorOptionsSnapshot)
            : this(execAdapter, logger, processorOptionsSnapshot.Value) { }

        public ExecutionProcessor(TAdapter execAdapter, ILogger logger, IExecutionProcessorOptions processorOptions)
        {
            this.execAdapter = execAdapter;
            this.logger = logger;
            this.processorOptions = processorOptions;
        }

        public Task<Core.Models.ExecutionContext> ProcessRequestAsync(ExecutionRequest execRequest, CancellationToken cancelToken)
        {
            if (execRequest == null)
            {
                throw new ArgumentNullException(nameof(execRequest));
            }

            logger.LogInformation($"Processing execution request [{execRequest.ExecutionId}]...");

            return execAdapter.ExecuteAsync(execRequest, cancelToken);
        }
    }
}
