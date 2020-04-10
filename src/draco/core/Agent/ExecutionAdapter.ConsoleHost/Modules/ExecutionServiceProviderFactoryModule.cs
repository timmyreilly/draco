// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.Interfaces;
using Core.Modules;
using Draco.Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Draco.ExecutionAdapter.ConsoleHost.Modules
{
    /// <summary>
    /// This module maps execution service names to execution service providers.
    /// For more information on execution services, see /doc/architecture/extension-services.md.
    /// </summary>
    public class ExecutionServiceProviderFactoryModule : BaseNamedServiceModule<IExecutionServiceProvider>
    {
        public override void AddNamedServices(IConfiguration configuration, INamedServiceRegistry<IExecutionServiceProvider> serviceRegistry)
        {
            // TODO: Register service providers here...
            // serviceRegistry["howdy/v1"] = sp => sp.GetService<HowdyServiceProvider>();
        }
    }
}
