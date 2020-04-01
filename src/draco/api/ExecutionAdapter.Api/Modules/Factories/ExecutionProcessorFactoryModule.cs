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
            serviceRegistry["stub/v1"] = sp => sp.GetService<ExecutionProcessor<StubExecutionAdapter>>(); // This is just a stub. It doesn't actually do anything. Replace before production.
        }
    }
}
