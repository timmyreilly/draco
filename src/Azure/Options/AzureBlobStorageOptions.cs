// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.Interfaces;

namespace Draco.Azure.Options
{
    public class AzureBlobStorageOptions : IAzureStorageAccountOptions
    {
        public string ConnectionString { get; set; }

        public bool CreateContainerIfNotExists { get; set; }
    }

    public class AzureBlobStorageOptions<T> : AzureBlobStorageOptions { }
}
