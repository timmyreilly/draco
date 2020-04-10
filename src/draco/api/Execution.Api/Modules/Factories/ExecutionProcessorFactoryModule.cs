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

namespace Draco.Execution.Api.Modules.Factories
{
    /// <summary>
    /// This service module maps execution models to execution processors that can handle them.
    /// As you onboard new execution models, you will need to configure the appropriate execution processors here.
    /// In the future, we will be adding support for dynamically registering new execution processors without having to modify/redeploy directly.
    /// For more information, see /doc/architecture/execution-pipeline.md.
    /// </summary>
    public class ExecutionProcessorFactoryModule : BaseNamedServiceModule<IExecutionProcessor>
    {
        public override void AddNamedServices(IConfiguration configuration, INamedServiceRegistry<IExecutionProcessor> serviceRegistry)
        {
            serviceRegistry[ExecutionModels.Async.Http.Json.V1] = sp => sp.GetService<ExecutionProcessor<IAsyncExecutionDispatcher>>(); // http-json/async/v1
            serviceRegistry[ExecutionModels.Sync.Http.Json.V1] = sp => sp.GetService<ExecutionProcessor<JsonHttpExecutionAdapter>>();   // http-json/sync/v1
        }
    }
}
