// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Hosting.Extensions;
using Draco.ExecutionAdapter.ConsoleHost.Modules;
using Draco.ExecutionAdapter.ConsoleHost.Modules.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Draco.ExecutionAdapter.ConsoleHost
{
    public class Program
    {
        public static async Task Main()
        {
            var host = new HostBuilder()
                .UseEnvironment(Environments.Development)
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    // Grab configuration information from blob storage...
                    // The information needed to access blob storage is provided through environment variables below.

                    var blobStorageAccount = CloudStorageAccount.Parse(BlobStorageConnectionString);
                    var blobClient = blobStorageAccount.CreateCloudBlobClient();
                    var blobContainer = blobClient.GetContainerReference(ContainerName);
                    var blob = blobContainer.GetBlockBlobReference(BlobName);
                    var configStream = new MemoryStream();

                    blob.DownloadToStream(configStream);

                    configStream.Position = 0;

                    configApp.AddJsonStream(configStream);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Register the subscriber hosted service...

                    services.AddHostedService<SubscriberHostedService>();

                    // Register core services...

                    services.ConfigureServices<CoreExecutionPipelineModule>(hostContext.Configuration)
                            .ConfigureServices<CoreObjectStorageModule>(hostContext.Configuration)
                            .ConfigureServices<ExecutionProcessorFactoryModule>(hostContext.Configuration)
                            .ConfigureServices<ExecutionServiceProviderFactoryModule>(hostContext.Configuration)
                            .ConfigureServices<InputObjectAccessorProviderFactoryModule>(hostContext.Configuration)
                            .ConfigureServices<OutputObjectAccessorProviderFactoryModule>(hostContext.Configuration)
                            .ConfigureServices<SignerModule>(hostContext.Configuration);

                    // Change these modules to re-platform...

                    services.ConfigureServices<AzureExecutionPipelineModule>(hostContext.Configuration)
                            .ConfigureServices<AzureObjectStorageModule>(hostContext.Configuration);

                    // Register execution adapters...

                    services.ConfigureServices<JsonHttpExecutionAdapterModule>(hostContext.Configuration);
                })
                .ConfigureLogging((hostContext, logConfig) =>
                {
                    logConfig.AddConsole();
                })
                .Build();

            await host.RunAsync();
        }

        private static string BlobStorageConnectionString { get; } =
           Environment.GetEnvironmentVariable("EXHUB_CONFIG_BLOB_STORAGE_CONNECTION_STRING");

        private static string ContainerName { get; } =
            Environment.GetEnvironmentVariable("EXHUB_CONFIG_BLOB_STORAGE_CONTAINER_NAME");

        private static string BlobName { get; } =
            Environment.GetEnvironmentVariable("EXHUB_CONFIG_BLOB_STORAGE_BLOB_NAME");
    }
}
