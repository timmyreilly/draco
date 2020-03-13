// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Draco.Core.Models.Extensions
{
    public static class ExecutionRequestExtensions
    {
        public static ExecutionContext ToExecutionContext(this ExecutionRequest execRequest) => new ExecutionContext
        {
            CreatedDateTimeUtc = execRequest.CreatedDateTimeUtc,
            Executor = execRequest.Executor,
            ExecutionId = execRequest.ExecutionId,
            ExecutionProfileName = execRequest.ExecutionProfileName,
            ExtensionId = execRequest.ExtensionId,
            ExtensionVersionId = execRequest.ExtensionVersionId,
            LastUpdatedDateTimeUtc = execRequest.LastUpdatedDateTimeUtc,
            Priority = execRequest.Priority,
            StatusUpdateKey = execRequest.StatusUpdateKey,
            ExecutionTimeoutDateTimeUtc = execRequest.ExecutionTimeoutDateTimeUtc,
            SupportedServices = execRequest.SupportedServices ?? new Dictionary<string, JObject>(),
            ExecutionModelName = execRequest.ExecutionModelName,
            ObjectProviderName = execRequest.ObjectProviderName,
            ProvidedInputObjects = execRequest.ProvidedInputObjects ?? new List<string>(),
            InputObjects = execRequest.InputObjects ?? new Dictionary<string, ExtensionInputObject>(),
            OutputObjects = execRequest.OutputObjects ?? new Dictionary<string, ExtensionOutputObject>(),
            ExecutorProperties = execRequest.ExecutorProperties ?? new Dictionary<string, string>()
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
