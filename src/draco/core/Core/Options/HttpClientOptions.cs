// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Interfaces;

namespace Draco.Core.Options
{
    /// <summary>
    /// Common options used by various HTTP clients (e.g., JsonHttpClient).
    /// </summary>
    /// <typeparam name="T">The target HTTP client type</typeparam>
    public class HttpClientOptions<T> : IHttpClientOptions
    {
        /// <summary>
        /// Gets/sets the maximum number of times an HTTP operation should be retried. 
        /// Typically, this value is used as part of an exponential backoff retry policy.
        /// </summary>
        /// <value>The maximum number of retries</value>
        public int MaximumRetryAttempts { get; set; } = 3;
    }
}
