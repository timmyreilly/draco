// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.Execution.Adapters;
using Draco.Core.Execution.Interfaces;
using Draco.Core.Execution.Processors;
using Draco.Core.Hosting.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Modules
{
    /// <summary>
    /// This service modules wires up all the stub dependencies needed to support the default execution pipeline in a dev/test environment.
    /// Note that this module should be used only in dev/test scenarios and is not meant for production.
    /// For more information on the execution pipeline, see /doc/architecture/execution-pipeline.md.
    /// </summary>
    public class StubExecutionPipelineModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IExecutionRequestRouter, ExecutionRequestRouter>();

            services.AddTransient<StubExecutionAdapter>();
            services.AddTransient<ExecutionProcessor<StubExecutionAdapter>>();
        }
    }
}