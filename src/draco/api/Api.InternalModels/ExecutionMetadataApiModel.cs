// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models.Enumerations;
using Newtonsoft.Json;

namespace Draco.Api.InternalModels
{
    /// <summary>
    /// Internal API model for core execution metadata -- src/draco/core/Core.Models/Interfaces/IExecutionMetadata.cs
    /// </summary>
    public class ExecutionMetadataApiModel
    {
        [JsonProperty("executionId")]
        public string ExecutionId { get; set; }

        [JsonProperty("extensionId")]
        public string ExtensionId { get; set; }

        [JsonProperty("extensionVersionId")]
        public string ExtensionVersionId { get; set; }

        [JsonProperty("executionProfileName")]
        public string ExecutionProfileName { get; set; }

        [JsonProperty("executor")]
        public ExecutorContextApiModel Executor { get; set; }

        [JsonProperty("priority")]
        public ExecutionPriority Priority { get; set; }
    }
}
