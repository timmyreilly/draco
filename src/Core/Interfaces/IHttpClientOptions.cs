// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Core.Interfaces
{
    public interface IHttpClientOptions
    {
        int MaximumRetryAttempts { get; }
    }
}
