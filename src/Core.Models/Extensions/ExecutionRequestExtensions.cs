// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Draco.Core.Models.Extensions
{
    public static class ExecutionRequestExtensions
    {
        public static ExecutionContext ToExecutionContext(this ExecutionRequest execRequest) => new ExecutionContext
        {
            CreatedDateTimeUtc = execRequest.CreatedDateTimeUtc,
            ExecutionId = execRequest.ExecutionId,
            ExecutionProfileName = execRequest.ExecutionProfileName,
            ExtensionId = execRequest.ExtensionId,
            ExtensionVersionId = execRequest.ExtensionVersionId,
            LastUpdatedDateTimeUtc = execRequest.LastUpdatedDateTimeUtc,
            Priority = execRequest.Priority,
            StatusUpdateKey = execRequest.StatusUpdateKey,
            ExecutionTimeoutDateTimeUtc = execRequest.ExecutionTimeoutDateTimeUtc,
            SupportedServices = execRequest.SupportedServices,
            ExecutionModelName = execRequest.ExecutionModelName,
            ObjectProviderName = execRequest.ObjectProviderName,
            ProvidedInputObjects = execRequest.ProvidedInputObjects,
            InputObjects = execRequest.InputObjects,
            OutputObjects = execRequest.OutputObjects,
            ExecutorProperties = execRequest.ExecutorProperties
        };

        public static ExecutionRequest CalculateExecutionTimeoutDateTimeUtc(this ExecutionRequest execRequest, TimeSpan defaultTimeoutPeriod)
        {
            if (execRequest == null)
            {
                throw new ArgumentNullException(nameof(execRequest));
            }

            execRequest.ExecutionTimeoutDateTimeUtc = DateTime.UtcNow.Add(
                execRequest.ExecutionTimeoutDuration ??
                defaultTimeoutPeriod);

            return execRequest;
        }
    }
}
