// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Interfaces;

namespace Draco.Core.Options
{
    public class HttpClientOptions<T> : IHttpClientOptions
    {
        public int MaximumRetryAttempts { get; set; } = 3;
    }
}
