// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.Interfaces;
using Core.Modules;
using Draco.Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Draco.Execution.Api.Modules.Factories
{
    /// <summary>
    /// This service module maps extension service names to extension service providers.
    /// As you onboard new extension services, you'll need to register them here.
    /// For more information, see /doc/architecture/extension-services.md.
    /// </summary>
    public class ExecutionServiceProviderFactoryModule : BaseNamedServiceModule<IExecutionServiceProvider>
    {
        public override void AddNamedServices(IConfiguration configuration, INamedServiceRegistry<IExecutionServiceProvider> serviceRegistry)
        {
            // Register new services here. See below for an example.
            // serviceRegistry["howdy/v1"] = sp => sp.GetService<HowdyServiceProvider>();
        }
    }
}
