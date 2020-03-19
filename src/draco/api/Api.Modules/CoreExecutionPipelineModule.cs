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

namespace Draco.Api.Modules
{
    /// <summary>
    /// This service module wires up all the dependencies needed for the core execution pipeline 
    /// including infrastrucutre needed to support the default "json-http/async/v1" and "json-http/sync/v1" 
    /// execution models (/doc/architecture/execution-models.md).
    /// 
    /// This service module is platform-agnostic and, typically, will need to be combined with additional platform-specific modules (Azure, etc.) 
    /// particularly as it relates to support for implementing the IAsyncExecutionDispatcher interface 
    /// (/src/draco/core/Core.Execution/Interfaces/IAsyncExecutionDispatcher.cs).
    /// 
    /// For more information on execution, see /doc/architecture/execution-models.md.
    /// </summary>
    public class CoreExecutionPipelineModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IExecutionServiceProvider, CompositeExecutionServiceProvider>();
            services.AddTransient<IExecutionRequestRouter, ExecutionRequestRouter>();

            services.AddTransient<ExecutionProcessor<IAsyncExecutionDispatcher>>();
            services.AddTransient<ExecutionProcessor<JsonHttpExecutionAdapter>>();
        }
    }
}
