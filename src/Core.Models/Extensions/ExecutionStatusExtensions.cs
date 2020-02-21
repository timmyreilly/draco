// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models.Enumerations;

namespace Draco.Core.Models.Extensions
{
    public static class ExecutionStatusExtensions
    {
        public static bool CanTransitionTo(this ExecutionStatus originalStatus, ExecutionStatus newStatus) =>
            (originalStatus == newStatus) ||
            (originalStatus == ExecutionStatus.Undefined) ||
            (originalStatus.IsPreProcessingStatus() && newStatus.IsPreProcessingStatus()) ||
            (originalStatus.IsPreProcessingStatus() && newStatus.IsProcessingStatus()) ||
            (originalStatus.IsProcessingStatus() && newStatus.IsPostProcessingStatus());

        public static bool IsPreProcessingStatus(this ExecutionStatus execStatus) =>
            (execStatus == ExecutionStatus.PendingInputObjects) ||
            (execStatus == ExecutionStatus.Queued) ||
            (execStatus == ExecutionStatus.Undefined);

        public static bool IsProcessingStatus(this ExecutionStatus execStatus) =>
            (execStatus == ExecutionStatus.Processing);

        public static bool IsPostProcessingStatus(this ExecutionStatus execStatus) =>
            (execStatus == ExecutionStatus.Canceled) ||
            (execStatus == ExecutionStatus.Failed) ||
            (execStatus == ExecutionStatus.Succeeded) ||
            (execStatus == ExecutionStatus.ValidationFailed) ||
            (execStatus == ExecutionStatus.ValidationSucceeded) ||
            (execStatus == ExecutionStatus.TimedOut);
    }
}
