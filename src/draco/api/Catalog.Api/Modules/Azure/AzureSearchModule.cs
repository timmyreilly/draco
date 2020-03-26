// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.Catalog.Services;
using Draco.Azure.Options;
using Draco.Core.Catalog.Interfaces;
using Draco.Core.Hosting.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.Catalog.Api.Modules.Azure
{
    /// <summary>
    /// This service module wires up all the configuration/dependencies needed to use Azure Search for full-text extension catalog search.
    /// </summary>
    public class AzureSearchModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ICatalogSearchService, AzureCatalogSearchService>();

            services.Configure<AzureSearchOptions<AzureCatalogSearchService>>(
                configuration.GetSection("platforms:azure:search:catalog"));
        }
    }
}
