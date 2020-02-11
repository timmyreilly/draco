using Draco.Azure.ObjectStorage.Interfaces;
using Draco.Azure.ObjectStorage.Models;
using Draco.Core.ObjectStorage.Enumerations;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Models;
using Draco.Core.ObjectStorage.Options;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Draco.Azure.ObjectStorage.Providers
{
    public class AzureOutputObjectUrlProvider : IOutputObjectUrlProvider
    {
        private readonly IAzureObjectStorageOptions storageOptions;
        private readonly IObjectUrlOptions urlOptions;
        private readonly IAzureObjectUrlProvider urlProvider;

        public AzureOutputObjectUrlProvider(
            IOptionsSnapshot<AzureObjectStorageOptions<AzureOutputObjectUrlProvider>> storageOptionsSnapshot,
            IOptionsSnapshot<ObjectUrlOptions<AzureOutputObjectUrlProvider>> urlOptionsSnapshot,
            IAzureObjectUrlProvider urlProvider)
            : this(storageOptionsSnapshot.Value, urlOptionsSnapshot.Value, urlProvider) { }

        public AzureOutputObjectUrlProvider(
            IAzureObjectStorageOptions storageOptions,
            IObjectUrlOptions urlOptions,
            IAzureObjectUrlProvider urlProvider)
        {
            this.storageOptions = storageOptions;
            this.urlOptions = urlOptions;
            this.urlProvider = urlProvider;
        }

        public async Task<ObjectUrl> GetReadableUrlAsync(ObjectUrlRequest urlRequest)
        {
            ValidateRequest(urlRequest);

            return await this.urlProvider.GetObjectUrlAsync(new AzureObjectUrlRequest
            {
                AccessMode = ObjectAccessMode.ReadOnly,
                BlobName = GetBlobName(urlRequest),
                ContainerName = storageOptions.ContainerName,
                ExpirationPeriod = GetExpirationPeriod(urlRequest),
                TenantId = urlRequest.ExecutionMetadata.Executor.TenantId
            });
        }

        public async Task<ObjectUrl> GetWritableUrlAsync(ObjectUrlRequest urlRequest)
        {
            ValidateRequest(urlRequest);

            return await this.urlProvider.GetObjectUrlAsync(new AzureObjectUrlRequest
            {
                AccessMode = ObjectAccessMode.WriteOnly,
                BlobName = GetBlobName(urlRequest),
                ContainerName = storageOptions.ContainerName,
                ExpirationPeriod = GetExpirationPeriod(urlRequest),
                TenantId = urlRequest.ExecutionMetadata.Executor.TenantId
            });
        }

        private void ValidateRequest(ObjectUrlRequest urlRequest)
        {
            if (urlRequest == null)
            {
                throw new ArgumentNullException(nameof(urlRequest));
            }

            if (urlRequest.ExecutionMetadata == null)
            {
                throw new ArgumentException($"{nameof(urlRequest.ExecutionMetadata)} is required.", nameof(urlRequest));
            }

            if (string.IsNullOrEmpty(urlRequest.ObjectName))
            {
                throw new ArgumentException($"{nameof(urlRequest.ObjectName)} is required.", nameof(urlRequest));
            }
        }

        private string GetBlobName(ObjectUrlRequest urlRequest) =>
            $"{urlRequest.ExecutionMetadata.ExecutionId}/output/{urlRequest.ObjectName}";

        private TimeSpan GetExpirationPeriod(ObjectUrlRequest urlRequest) =>
            (urlRequest.UrlExpirationPeriod ?? urlOptions.DefaultUrlExpirationPeriod);
    }
}
