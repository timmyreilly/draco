// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core;
using Draco.Core.Execution.Adapters;
using Draco.Core.Hosting.Interfaces;
using Draco.Core.Interfaces;
using Draco.Core.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ExecutionAdapter.ConsoleHost.Modules
{
    /// <summary>
    /// This module wires up all the configuration/dependencies needed to support the "json-http/async/v1" execution model.
    /// For more information on execution models, see /doc/architecture/execution-models.md.
    /// For more information on the execution pipeline and the role that execution adapters play, see /doc/architecture/execution-pipeline.md.
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
