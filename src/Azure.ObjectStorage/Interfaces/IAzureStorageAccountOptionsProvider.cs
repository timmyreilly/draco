// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.Interfaces;
using System.Threading.Tasks;

namespace Draco.Azure.ObjectStorage.Interfaces
{
    public interface IAzureStorageAccountOptionsProvider
    {
        Task<IAzureStorageAccountOptions> GetStorageAccountOptionsAsync(string tenantId);
    }
}
