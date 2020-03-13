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
    public class ExecutionProcessorFactoryModule : BaseNamedServiceModule<IExecutionProcessor>
    {
        public override void AddNamedServices(IConfiguration configuration, INamedServiceRegistry<IExecutionProcessor> serviceRegistry)
        {
            serviceRegistry[ExecutionModels.Async.Http.Json.V1] = sp => sp.GetService<ExecutionProcessor<IAsyncExecutionDispatcher>>();
            serviceRegistry[ExecutionModels.Sync.Http.Json.V1] = sp => sp.GetService<ExecutionProcessor<JsonHttpExecutionAdapter>>();
        }
    }
}
