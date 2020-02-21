// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Core.Models.Enumerations
{
    public enum ExecutionStatus
    {
        Undefined = 0,
        PendingInputObjects,
        Queued,
        Processing,
        ValidationFailed,
        ValidationSucceeded,
        Succeeded,
        Failed,
        Canceled,
        TimedOut,
        DirectExecutionTokenProvided
    }
}
