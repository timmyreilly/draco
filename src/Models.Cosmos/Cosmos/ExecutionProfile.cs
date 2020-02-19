// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models.Enumerations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Draco.Azure.Models.Cosmos
{
    public class ExecutionProfile
    {
        [JsonProperty("profileDescription")]
        public string ProfileDescription { get; set; }

        [JsonProperty("executionModelName")]
        public string ExecutionModelName { get; set; }

        [JsonProperty("objectProviderName")]
        public string ObjectProviderName { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("baseExecutionCost")]
        public double? BaseExecutionCost { get; set; }

        [JsonProperty("executionMode")]
        public ExecutionMode ExecutionMode { get; set; }

        [JsonProperty("supportedPriorities")]
        public ExecutionPriority SupportedPriorities { get; set; }

        [JsonProperty("directExecutionTokenDuration")]
        public TimeSpan? DirectExecutionTokenDuration { get; set; }

        [JsonProperty("clientConfiguration")]
        public JObject ClientConfiguration { get; set; }

        [JsonProperty("extensionSettings")]
        public JObject ExtensionSettings { get; set; }
    }
}
