// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.Execution;
using Draco.Azure.Options;
using Draco.Core.Execution.Interfaces;
using Draco.Core.Hosting.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.Execution.Api.Modules.Azure
{
    /// <summary>
    /// This service module wires up all the configuration/depenenices needed to host an execution pipeline on Azure.
    /// For more information on the execution pipeline, see /doc/architecture/execution-pipeline.md.
    /// </summary>
    public class AzureExecutionPipelineModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IAsyncExecutionDispatcher, ServiceBusExecutionAdapter>();
            services.AddTransient<IExecutionUpdatePublisher, EventGridExecutionUpdatePublisher>();

            services.Configure<ServiceBusTopicOptions<ServiceBusExecutionAdapter>>(
                configuration.GetSection("platforms:azure:executionPipeline:serviceBus:executionAdapter"));

            services.Configure<EventGridTopicOptions<EventGridExecutionUpdatePublisher>>(
                configuration.GetSection("platforms:azure:executionPipeline:eventGrid:updatePublisher"));
        }
    }
}
