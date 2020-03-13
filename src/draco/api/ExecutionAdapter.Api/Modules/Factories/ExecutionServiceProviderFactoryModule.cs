// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.Interfaces;
using Core.Modules;
using Core.Services.Providers;
using Draco.Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ExecutionAdapter.Api.Modules.Factories
{
    public class ExecutionServiceProviderFactoryModule : BaseNamedServiceModule<IExecutionServiceProvider>
    {
        public override void AddNamedServices(IConfiguration configuration, INamedServiceRegistry<IExecutionServiceProvider> serviceRegistry)
        {
            serviceRegistry["stub/v1"] = sp => sp.GetService<StubExecutionServiceProvider>();
        }
    }
}
