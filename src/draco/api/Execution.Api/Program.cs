// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Draco.Execution.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(cb =>
                {
                    // Pull execution API configuration from Azure blob storage.
                    // All the informaiton needed to access the right storage account is passed in through environment variables (see below).

                    var blobStorageAccount = CloudStorageAccount.Parse(BlobStorageConnectionString);
                    var blobClient = blobStorageAccount.CreateCloudBlobClient();
                    var blobContainer = blobClient.GetContainerReference(ContainerName);
                    var blob = blobContainer.GetBlockBlobReference(BlobName);
                    var configStream = new MemoryStream();

                    blob.DownloadToStream(configStream);

                    configStream.Position = 0;

                    cb.AddJsonStream(configStream);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static string BlobStorageConnectionString { get; } =
            Environment.GetEnvironmentVariable("EXHUB_CONFIG_BLOB_STORAGE_CONNECTION_STRING");

        private static string ContainerName { get; } =
            Environment.GetEnvironmentVariable("EXHUB_CONFIG_BLOB_STORAGE_CONTAINER_NAME");

        private static string BlobName { get; } =
            Environment.GetEnvironmentVariable("EXHUB_CONFIG_BLOB_STORAGE_BLOB_NAME");
    }
}
