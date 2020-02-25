// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Core.Models.Extensions
{
    public static class ExecutionExtensions
    {
        public static ExecutionUpdateEvent ToEvent(this Execution execution) =>
            new ExecutionUpdateEvent
            {
                ExecutionId = execution.ExecutionId,
                Executor = execution.Executor,
                ExtensionId = execution.ExtensionId,
                ExtensionVersionId = execution.ExtensionVersionId,
                PercentageComplete = execution.PercentComplete,
                Priority = execution.Priority,
                Status = execution.Status,
                StatusMessage = execution.StatusMessage,
                UpdateDateTimeUtc = execution.LastUpdatedDateTimeUtc,
                ExecutorProperties = execution.ExecutorProperties
            };
    }
}
