// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Core.Execution.Interfaces
{
    public interface IExecutionAdapterOptions
    {
        int MaximumRetryAttempts { get; }
    }
}
