// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Azure.Interfaces
{
    public interface IAzureSearchOptions
    {
        string IndexName { get; }
        string QueryKey { get; }
        string ServiceName { get; }
    }
}
