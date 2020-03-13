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