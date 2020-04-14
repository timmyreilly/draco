// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Core.Execution.Interfaces
{
    /// <summary>
    /// Defines common, generic options used by execution adapters.
    /// </summary>
    public interface IExecutionAdapterOptions
    {
        int MaximumRetryAttempts { get; }
    }
}
