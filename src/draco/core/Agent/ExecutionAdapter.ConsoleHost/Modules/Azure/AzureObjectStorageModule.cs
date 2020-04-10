// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.ObjectStorage.Interfaces;
using Draco.Azure.ObjectStorage.Providers;
using Draco.Azure.Options;
using Draco.Core.Hosting.Interfaces;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ExecutionAdapter.ConsoleHost.Modules.Azure
{
    /// <summary>
    /// This modules wires up all the configuration/dependenices needed to use Azure blob storage and shared access signatures (SAS)
    /// for input/output object accessors. For more infomraiton on Azure blob storage SAS, see https://docs.microsoft.com/en-us/azure/storage/common/storage-sas-overview.
    /// For more information on execution objects, see /doc/architecture/execution-objects.md.
    /// </summary>
    public class AzureObjectStorageModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IInputObjectUrlProvider, AzureInputObjectUrlProvider>();
            services.AddTransient<IOutputObjectUrlProvider, AzureOutputObjectUrlProvider>();
            services.AddTransient<IAzureObjectUrlProvider, AzureObjectUrlProvider>();
            services.AddTransient<IAzureStorageAccountOptionsProvider, DefaultAzureStorageAccountOptionsProvider>();

            services.Configure<AzureObjectStorageOptions<AzureInputObjectUrlProvider>>(
                configuration.GetSection("platforms:azure:objectStorage:blobStorage:inputObjectUrlProvider"));

            services.Configure<AzureObjectStorageOptions<AzureOutputObjectUrlProvider>>(
                configuration.GetSection("platforms:azure:objectStorage:blobStorage:outputObjectUrlProvider"));

            services.Configure<AzureBlobStorageOptions>(
                configuration.GetSection("platforms:azure:objectStorage:blobStorage:storageAccount"));

            services.Configure<ObjectUrlOptions<AzureInputObjectUrlProvider>>(
                configuration.GetSection("core:objectStorage:url"));

            services.Configure<ObjectUrlOptions<AzureOutputObjectUrlProvider>>(
                configuration.GetSection("core:objectStorage:url"));
        }
    }
}
