// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.Interfaces;
using Core.Modules;
using Draco.Core.Services.Interfaces;
using Draco.IntegrationTests.HowdyService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ExecutionAdapter.ConsoleHost.Modules
{
    public class ExecutionServiceProviderFactoryModule : BaseNamedServiceModule<IExecutionServiceProvider>
    {
        public override void AddNamedServices(IConfiguration configuration, INamedServiceRegistry<IExecutionServiceProvider> serviceRegistry)
        {
            serviceRegistry["howdy/v1"] = sp => sp.GetService<HowdyServiceProvider>();
        }
    }
}
