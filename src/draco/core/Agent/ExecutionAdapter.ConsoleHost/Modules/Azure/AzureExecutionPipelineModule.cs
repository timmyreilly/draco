// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.Execution;
using Draco.Azure.Options;
using Draco.Core.Execution.Interfaces;
using Draco.Core.Hosting.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ExecutionAdapter.ConsoleHost.Modules.Azure
{
    /// <summary>
    /// This module wires up all the configuration/dependencies needed to use an Azure Cosmos DB extension repository.
    /// </summary>
    public class AzureExecutionPipelineModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IExecutionRequestSubscriber, ServiceBusExecutionRequestSubscriber>();

            services.Configure<ServiceBusSubscriptionOptions<ServiceBusExecutionRequestSubscriber>>(
                configuration.GetSection("platforms:azure:executionPipeline:serviceBus:requestSubscriber"));
        }
    }
}
