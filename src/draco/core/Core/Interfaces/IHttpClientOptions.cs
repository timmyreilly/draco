// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Core.Interfaces
{
    /// <summary>
    /// Defines common options used by various HTTP clients (e.g., IJsonHttpClient).
    /// </summary>
    public interface IHttpClientOptions
    {
        /// <summary>
        /// Gets the maximum number of times an HTTP operation should be retried.
        /// Typically, this value is used as part of an exponential backoff retry policy.
        /// </summary>
        /// <value>The maximum number of retries</value>
        int MaximumRetryAttempts { get; }
    }
}
