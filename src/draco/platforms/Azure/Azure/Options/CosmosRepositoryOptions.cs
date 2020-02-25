// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.Interfaces;

namespace Draco.Azure.Options
{
    public class CosmosRepositoryOptions : ICosmosRepositoryOptions
    {
        public string EndpointUri { get; set; }
        public string AccessKey { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }

    public class CosmosRepositoryOptions<T> : CosmosRepositoryOptions { }
}
