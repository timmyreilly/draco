// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core;
using Draco.Core.Execution.Adapters;
using Draco.Core.Hosting.Interfaces;
using Draco.Core.Interfaces;
using Draco.Core.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.Api.Modules
{
    /// <summary>
    /// This service module wires up all the configuration/dependencies needed to support
    /// the "http-json/async/v1" and "http-json/sync/v1" execution models.
    /// </summary>
    public class JsonHttpExecutionAdapterModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<JsonHttpExecutionAdapter>();
            services.AddTransient<IJsonHttpClient, JsonHttpClient>();

            services.Configure<HttpClientOptions<JsonHttpClient>>(
                configuration.GetSection("core:executionPipeline:httpClient"));
        }
    }
}
