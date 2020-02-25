// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.Interfaces;
using Draco.Azure.ObjectStorage.Interfaces;
using Draco.Azure.Options;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Draco.Azure.ObjectStorage.Providers
{
    public class DefaultAzureStorageAccountOptionsProvider : IAzureStorageAccountOptionsProvider
    {
        private readonly IAzureStorageAccountOptions storageAccountOptions;

        public DefaultAzureStorageAccountOptionsProvider(IOptionsSnapshot<AzureBlobStorageOptions> optionsSnapshot)
        {
            this.storageAccountOptions = optionsSnapshot.Value;
        }

        public Task<IAzureStorageAccountOptions> GetStorageAccountOptionsAsync(string tenantId)
        {
            if (string.IsNullOrEmpty(tenantId))
                throw new ArgumentNullException(nameof(tenantId));

            return Task.FromResult(storageAccountOptions);
        }
    }
}
