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
    /// This service module wires up all the dependencies needed to stand up a "stub" execution pipeline. 
    /// It has no external service dependencies and should be used ONLY in execution pipeline integration/end-to-end testing scenarios.
    /// For more information on execution, see /doc/architecture/execution-models.md.
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