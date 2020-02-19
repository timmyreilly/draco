// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Factories;
using Draco.Core.Hosting.Interfaces;
using Draco.Core.Interfaces;
using Draco.Core.Services.Interfaces;
using Draco.IntegrationTests.HowdyService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.Execution.Api.Modules.Factories
{
    public class ExecutionServiceProviderFactoryModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<INamedServiceFactory<IExecutionServiceProvider>>(
               new NamedServiceFactory<IExecutionServiceProvider>
               {
                   ["howdy/v1"] = sp => sp.GetService<HowdyServiceProvider>()

                   // Plug in additional extension services here...
               });
        }
    }
}
