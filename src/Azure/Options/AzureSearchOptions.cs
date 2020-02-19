// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.Interfaces;

namespace Draco.Azure.Options
{
    public class AzureSearchOptions : IAzureSearchOptions
    {
        public string IndexName { get; set; }
        public string QueryKey { get; set; }
        public string ServiceName { get; set; }
    }

    public class AzureSearchOptions<T> : AzureSearchOptions { }
}
