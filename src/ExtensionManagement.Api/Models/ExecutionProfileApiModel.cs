// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Draco.ExtensionManagement.Api.Models
{
    public class ExecutionProfileApiModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("extensionId")]
        public string ExtensionId { get; set; }

        [JsonProperty("extensionVersionId")]
        public string ExtensionVersionId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("executionModel")]
        public string ExecutionModelName { get; set; }

        [JsonProperty("objectProvider")]
        public string ObjectProviderName { get; set; }

        [JsonProperty("executionMode")]
        public string ExecutionMode { get; set; }

        [JsonProperty("directExecutionTokenDuration")]
        public string DirectExecutionTokenDuration { get; set; }

        [JsonProperty("supportedPriorities")]
        public List<string> SupportedPriorities { get; set; } = new List<string>();

        [JsonProperty("clientConfiguration")]
        public Dictionary<string, string> ClientConfiguration { get; set; } = new Dictionary<string, string>();

        [JsonProperty("extensionSettings")]
        public Dictionary<string, string> ExtensionSettings { get; set; } = new Dictionary<string, string>();
    }
}
