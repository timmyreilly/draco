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

            if (processorFactory.ContainsKey(execRequest.ExecutionModelName) == false)
            {
                throw new NotSupportedException($"Execution model [{execRequest.ExecutionModelName}] not supported.");
            }

            var processor = processorFactory[execRequest.ExecutionModelName](serviceProvider);

            return processor.ProcessRequestAsync(execRequest, cancelToken);
        }
    }
}
