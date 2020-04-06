// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Polly;
using Polly.Retry;
using System;

namespace Draco.Core.Extensions
{
    /// <summary>
    /// Extension methods for qucickly building Polly retry policies.
    /// </summary>
    public static class PolicyBuilderExtensions
    {
        /// <summary>
        /// Create a standard exponential back-off Polly retry policy.
        /// </summary>
        /// <param name="policyBuilder"></param>
        /// <param name="retryAttempts"></param>
        /// <returns></returns>
        public static RetryPolicy ConfigureExponentialBackOffRetryPolicy(this PolicyBuilder policyBuilder, int retryAttempts = 3) =>
            policyBuilder.WaitAndRetry(retryAttempts, ra => TimeSpan.FromSeconds(Math.Pow(2, ra)));
    }
}
