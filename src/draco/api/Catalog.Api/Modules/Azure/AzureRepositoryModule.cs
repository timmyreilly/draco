// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.Models.Cosmos.Repositories;
using Draco.Azure.Options;
using Draco.Core.Hosting.Interfaces;
using Draco.Core.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.Catalog.Api.Modules.Azure
{
    /// <summary>
    /// This service modules up all the configuration/dependencies needed to use Azure Cosmos DB for the extension repository.
    /// </summary>
    public class AzureRepositoryModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IExtensionRepository, CosmosExtensionRepository>();

            services.Configure<CosmosRepositoryOptions<CosmosExtensionRepository>>(
                configuration.GetSection("platforms:azure:repositories:cosmosDb:extension"));
        }
    }
}