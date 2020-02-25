// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models.Enumerations;
using Newtonsoft.Json;

namespace Draco.Api.InternalModels
{
    public class ExecutionMetadataApiModel
    {
        [JsonProperty("executionId")]
        public string ExecutionId { get; set; }

        [JsonProperty("extensionId")]
        public string ExtensionId { get; set; }

        [JsonProperty("extensionVersionId")]
        public string ExtensionVersionId { get; set; }

        [JsonProperty("extensionProfileName")]
        public string ExecutionProfileName { get; set; }

        [JsonProperty("executor")]
        public ExecutorContextApiModel Executor { get; set; }

        [JsonProperty("priority")]
        public ExecutionPriority Priority { get; set; }
    }
}
