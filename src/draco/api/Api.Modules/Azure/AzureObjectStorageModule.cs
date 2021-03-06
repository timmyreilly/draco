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

namespace Draco.Api.Modules.Azure
{
    /// <summary>
    /// Default service module for wiring up all the configuration/dependencies needed for using
    /// the default Azure blobs (az-blobs/v1) object storage provider
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
