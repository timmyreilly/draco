// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Interfaces;
using Draco.Core.Interfaces;
using Draco.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Draco.Core.Execution.Processors
{
    /// <summary>
    /// This is the default execution request router that routes execution requests to the appropriate
    /// processor based on execution model name. For more information on the execution request router and the
    /// role that it plays in the Draco execution pipeline, see /doc/architecture/execution-pipeline.md.
    /// </summary>
    public class ExecutionRequestRouter : IExecutionRequestRouter
    {
        private readonly INamedServiceFactory<IExecutionProcessor> processorFactory;
        private readonly IServiceProvider serviceProvider;

        public ExecutionRequestRouter(INamedServiceFactory<IExecutionProcessor> processorFactory,
                                      IServiceProvider serviceProvider)
        {
            this.processorFactory = processorFactory;
            this.serviceProvider = serviceProvider;
        }

        public Task<Core.Models.ExecutionContext> RouteRequestAsync(ExecutionRequest execRequest, CancellationToken cancelToken)
        {
            if (execRequest == null)
            {
                throw new ArgumentNullException(nameof(execRequest));
            }

            // If this hub doesn't support a specific execution model, throw an exception and let the
            // execution API communicate the failure back to the client appropriately.

            if (processorFactory.CanCreateService(execRequest.ExecutionModelName) == false)
            {
                throw new NotSupportedException($"Execution model [{execRequest.ExecutionModelName}] not supported.");
            }

            // Otherwise, create the processor and dispatch the execution request.

            var processor = processorFactory.CreateService(execRequest.ExecutionModelName, serviceProvider);

            return processor.ProcessRequestAsync(execRequest, cancelToken);
        }
    }
}
