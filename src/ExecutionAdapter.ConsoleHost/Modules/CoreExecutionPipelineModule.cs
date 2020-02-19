// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Adapters;
using Draco.Core.Execution.Interfaces;
using Draco.Core.Execution.Processors;
using Draco.Core.Hosting.Interfaces;
using Draco.Core.Services.Interfaces;
using Draco.Core.Services.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ExecutionAdapter.ConsoleHost.Modules
{
    public class CoreExecutionPipelineModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IExecutionServiceProvider, CompositeExecutionServiceProvider>();
            services.AddTransient<IExecutionRequestRouter, ExecutionRequestRouter>();

            services.AddTransient<AsyncExecutionProcessor<JsonHttpExecutionAdapter>>();
        }
    }
}
