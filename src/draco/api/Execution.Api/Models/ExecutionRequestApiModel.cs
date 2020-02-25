// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Constants;
using Draco.Core.Models.Enumerations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Draco.Execution.Api.Models
{
    public class ExecutionRequestApiModel
    {
        [JsonProperty("extensionId")]
        public string ExtensionId { get; set; }

        [JsonProperty("extensionVersionId")]
        public string ExtensionVersionId { get; set; }

        [JsonProperty("profileName")]
        public string ProfileName { get; set; } = ExecutionProfiles.Default;

        [JsonProperty("priority")]
        public string Priority { get; set; } = ExecutionPriority.Normal.ToString();

        [JsonProperty("properties")]
        public Dictionary<string, string> ExecutorProperties { get; set; } = new Dictionary<string, string>();

        [JsonProperty("validateOnly")]
        public bool ValidateOnly { get; set; }

        [JsonProperty("requestData")]
        public JObject RequestData { get; set; }
    }
}
