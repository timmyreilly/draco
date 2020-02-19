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
