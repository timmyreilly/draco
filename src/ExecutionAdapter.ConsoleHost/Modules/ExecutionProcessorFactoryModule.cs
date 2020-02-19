// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Adapters;
using Draco.Core.Execution.Constants;
using Draco.Core.Execution.Interfaces;
using Draco.Core.Execution.Processors;
using Draco.Core.Factories;
using Draco.Core.Hosting.Interfaces;
using Draco.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ExecutionAdapter.ConsoleHost.Modules
{
    public class ExecutionProcessorFactoryModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<INamedServiceFactory<IExecutionProcessor>>(
                new NamedServiceFactory<IExecutionProcessor>
                {
                    [ExecutionModels.Async.Http.Json.V1] = sp => sp.GetService<AsyncExecutionProcessor<JsonHttpExecutionAdapter>>()

                    // Plug in additional execution adapters here...
                });
        }
    }
}
