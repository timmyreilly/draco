// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Polly;
using Polly.Retry;
using System;

namespace Draco.Core.Extensions
{
    public static class PolicyBuilderExtensions
    {
        public static RetryPolicy ConfigureExponentialBackOffRetryPolicy(this PolicyBuilder policyBuilder, int retryAttempts = 3) =>
            policyBuilder.WaitAndRetry(retryAttempts, ra => TimeSpan.FromSeconds(Math.Pow(2, ra)));
    }
}
