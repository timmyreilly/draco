// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.ObjectStorage.Interfaces;
using Draco.Azure.ObjectStorage.Models;
using Draco.Core.ObjectStorage.Enumerations;
using Draco.Core.ObjectStorage.Models;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Draco.Azure.ObjectStorage.Providers
{
    public class AzureObjectUrlProvider : IAzureObjectUrlProvider
    {
        private readonly IAzureStorageAccountOptionsProvider storageAccountOptionsProvider;

        public AzureObjectUrlProvider(IAzureStorageAccountOptionsProvider storageAccountOptionsProvider)
        {
            this.storageAccountOptionsProvider = storageAccountOptionsProvider;
        }

        public async Task<ObjectUrl> GetObjectUrlAsync(AzureObjectUrlRequest urlRequest)
        {
            ValidateUrlRequest(urlRequest);

            var storageOptions = await storageAccountOptionsProvider.GetStorageAccountOptionsAsync(urlRequest.TenantId);
            var storageAccount = CloudStorageAccount.Parse(storageOptions.ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(urlRequest.ContainerName.ToLower());
            var blob = blobContainer.GetBlockBlobReference(urlRequest.BlobName);
            var expirationDateTimeUtc = DateTime.UtcNow.Add(urlRequest.ExpirationPeriod);
            var accessPolicy = new SharedAccessBlobPolicy { SharedAccessExpiryTime = expirationDateTimeUtc };

            if (urlRequest.AccessMode == ObjectAccessMode.ReadOnly)
            {
                accessPolicy.Permissions = SharedAccessBlobPermissions.Read;

                return new ObjectUrl
                {
                    ExpirationDateTimeUtc = expirationDateTimeUtc,
                    HttpMethod = HttpMethod.Get.Method,
                    Url = (blob.Uri + blob.GetSharedAccessSignature(accessPolicy, null, null, SharedAccessProtocol.HttpsOnly, null)),
                    AccessMode = ObjectAccessMode.ReadOnly.ToString()
                };
            }
            else
            {
                accessPolicy.Permissions = SharedAccessBlobPermissions.Create | SharedAccessBlobPermissions.Write;

                return new ObjectUrl
                {
                    ExpirationDateTimeUtc = expirationDateTimeUtc,
                    HttpMethod = HttpMethod.Put.Method,
                    RequestHeaders = new Dictionary<string, string> { ["x-ms-blob-type"] = "BlockBlob" },
                    Url = (blob.Uri + blob.GetSharedAccessSignature(accessPolicy, null, null, SharedAccessProtocol.HttpsOnly, null)),
                    AccessMode = ObjectAccessMode.WriteOnly.ToString()
                };
            }
        }

        private void ValidateUrlRequest(AzureObjectUrlRequest urlRequest)
        {
            if (urlRequest == null)
            {
                throw new ArgumentNullException(nameof(urlRequest));
            }

            if (string.IsNullOrEmpty(urlRequest.ContainerName))
            {
                throw new ArgumentException($"{nameof(urlRequest.ContainerName)} is required.", nameof(urlRequest));
            }

            if (string.IsNullOrEmpty(urlRequest.BlobName))
            {
                throw new ArgumentException($"{nameof(urlRequest.BlobName)} is required.", nameof(urlRequest));
            }

            if (string.IsNullOrEmpty(urlRequest.TenantId))
            {
                throw new ArgumentException($"{nameof(urlRequest.TenantId)} is required.", nameof(urlRequest));
            }
        }
    }
}
