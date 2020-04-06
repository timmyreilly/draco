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
    /// <summary>
    /// Wires up all the platform-agnostic core dependencies needed to power the execution pipeline with the standard
    /// "http-json/async/v1" execution model. For more information on execution models, see /doc/architecture/execution-models.md.
    /// For more information on the execution pipeline, see /doc/architecture/execution-pipeline.md.
    /// </summary>
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
