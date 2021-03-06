// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.Models.Cosmos.Repositories;
using Draco.Azure.Options;
using Draco.Core.Hosting.Interfaces;
using Draco.Core.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.Execution.Api.Modules.Azure
{
    /// <summary>
    /// This service module provides all the configuration/dependencies needed to host both the
    /// execution and extension repositories in Azure Cosmos DB.
    /// </summary>
    public class AzureRepositoryModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IExecutionRepository, CosmosExecutionRepository>();
            services.AddTransient<IExtensionRepository, CosmosExtensionRepository>();

            services.Configure<CosmosRepositoryOptions<CosmosExecutionRepository>>(
                configuration.GetSection("platforms:azure:repositories:cosmosDb:execution"));

            services.Configure<CosmosRepositoryOptions<CosmosExtensionRepository>>(
                configuration.GetSection("platforms:azure:repositories:cosmosDb:extension"));
        }
    }
}
