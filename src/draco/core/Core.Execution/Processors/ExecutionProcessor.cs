// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
    /// <summary>
    /// This processor invokes the appropriate execution adapter [TAdapter] to execute the target extension
    /// and reports execution updates directly back to the execution API in-process. Typically, this processor is used in
    /// synchronous execution scenarios as part of the execution or execution adapter APIs. For more information on execution processors and the
    /// role that they play in the Draco execution pipeline, see /doc/architecture/execution-pipeline.md.
    /// </summary>
    /// <typeparam name="TAdapter">The type of execution adapter that this processor uses to execute an extension</typeparam>
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

            // We're not doing much here. In the case of synchronous execution, the processor is really just a lightweight "coupling"
            // that connects the execution or execution adapter API to the execution adapter.

            logger.LogInformation($"Processing execution request [{execRequest.ExecutionId}]...");

            return execAdapter.ExecuteAsync(execRequest, cancelToken);
        }
    }
}
