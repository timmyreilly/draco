using Draco.Core.Hosting.Interfaces;
using Draco.Execution.Api.Interfaces;
using Draco.Execution.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.Execution.Api.Modules
{
    public class DefaultUserContextModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IUserContext>(
                new UserContext
                {
                    Executor = new Core.Models.ExecutorContext
                    {
                        TenantId = "598f9af9-f226-41b4-bd82-a2944cb1f1b1",
                        UserId = "2e43f69d-f8eb-429e-800a-f61ba7790f26"
                    }
                });
        }
    }
}
