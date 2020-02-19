// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Azure.Interfaces
{
    public interface ICosmosRepositoryOptions
    {
        string EndpointUri { get; }
        string AccessKey { get; }
        string DatabaseName { get; }
        string CollectionName { get; }
    }
}
