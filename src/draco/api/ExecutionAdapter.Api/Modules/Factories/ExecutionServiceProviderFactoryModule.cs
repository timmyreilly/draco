// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.Interfaces;
using Core.Modules;
using Draco.Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Draco.ExecutionAdapter.Api.Modules.Factories
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
            // Add execution service providers here. Example below.
            // serviceRegistry["stub/v1"] = sp => sp.GetService<StubExecutionServiceProvider>();
        }
    }
}
