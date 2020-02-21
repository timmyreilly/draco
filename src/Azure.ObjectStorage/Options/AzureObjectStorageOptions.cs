// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.ObjectStorage.Interfaces;

namespace Draco.Azure.ObjectStorage.Providers
{
    public class AzureObjectStorageOptions : IAzureObjectStorageOptions
    {
        public string ContainerName { get; set; }
    }

    public class AzureObjectStorageOptions<T> : AzureObjectStorageOptions { }
}
