using Draco.Core.Hosting.Interfaces;
using Draco.IntegrationTests.HowdyService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ExecutionAdapter.ConsoleHost.Modules.ExecutionServices
{
    public class HowdyExecutionServiceModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<HowdyServiceProvider>();
        }
    }
}
