using Draco.Core;
using Draco.Core.Execution.Adapters;
using Draco.Core.Hosting.Interfaces;
using Draco.Core.Interfaces;
using Draco.Core.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ExecutionAdapter.ConsoleHost.Modules
{
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
