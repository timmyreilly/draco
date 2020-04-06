// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.Interfaces;
using Core.Modules;
using Draco.Core.Execution.Adapters;
using Draco.Core.Execution.Constants;
using Draco.Core.Execution.Interfaces;
using Draco.Core.Execution.Processors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ExecutionAdapter.ConsoleHost.Modules
{
    /// <summary>
    /// Maps execution model names to execution processors + execution adapters that can handle them.
    /// For more information on execution models, see /doc/architecture/execution-models.md.
    /// For more information on the execution pipeline and the role that execution adapters play, see /doc/architecture/execution-pipeline.md.
    /// </summary>
    public class ExecutionProcessorFactoryModule : BaseNamedServiceModule<IExecutionProcessor>
    {
        public override void AddNamedServices(IConfiguration configuration, INamedServiceRegistry<IExecutionProcessor> serviceRegistry)
        {
            // Out of the box, this agent is configured to support only the "json-http/async/v1" execution model.
            // Note the use of the AsyncExecutionProcessor which, unlike the standard ExecutionProcessors used in the execution API, HTTP POSTs 
            // execution status updates back to the execution API.

            serviceRegistry[ExecutionModels.Async.Http.Json.V1] = sp => sp.GetService<AsyncExecutionProcessor<JsonHttpExecutionAdapter>>();
        }
    }
}
