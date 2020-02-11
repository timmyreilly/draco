using Draco.Azure.ObjectStorage.Constants;
using Draco.Core.Factories;
using Draco.Core.Hosting.Interfaces;
using Draco.Core.Interfaces;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ExecutionAdapter.ConsoleHost.Modules
{
    public class ObjectAccessorProviderFactoryModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<INamedServiceFactory<IInputObjectAccessorProvider>>(
                new NamedServiceFactory<IInputObjectAccessorProvider>
                {
                    [AzureObjectStorageProviders.BlobStorage.V1] = sp => sp.GetService<InputObjectUrlAccessorProvider>()
                });

            services.AddSingleton<INamedServiceFactory<IOutputObjectAccessorProvider>>(
                new NamedServiceFactory<IOutputObjectAccessorProvider>
                {
                    [AzureObjectStorageProviders.BlobStorage.V1] = sp => sp.GetService<OutputObjectUrlAccessorProvider>()
                });
        }
    }
}
