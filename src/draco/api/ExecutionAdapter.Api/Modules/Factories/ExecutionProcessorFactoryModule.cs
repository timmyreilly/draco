// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.Execution.Adapters;
using Core.Interfaces;
using Core.Modules;
using Draco.Core.Execution.Interfaces;
using Draco.Core.Execution.Processors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ExecutionAdapter.Api.Modules.Factories
{
    public class ExecutionProcessorFactoryModule : BaseNamedServiceModule<IExecutionProcessor>
    {
        public override void AddNamedServices(IConfiguration configuration, INamedServiceRegistry<IExecutionProcessor> serviceRegistry)
        {
            serviceRegistry["stub/v1"] = sp => sp.GetService<ExecutionProcessor<StubExecutionAdapter>>();
        }
    }
}
