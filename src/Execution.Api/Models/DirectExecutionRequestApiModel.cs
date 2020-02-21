// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Draco.Execution.Api.Models
{
    public class DirectExecutionRequestApiModel
    {
        public string ExecutionId { get; set; }
        [JsonProperty("extensionId")]
        public string ExtensionId { get; set; }

        [JsonProperty("extensionVersionId")]
        public string ExtensionVersionId { get; set; }

        [JsonProperty("executionModel")]
        public string ExecutionModelName { get; set; }

        [JsonProperty("executionProfile")]
        public string ExecutionProfileName { get; set; }

        [JsonProperty("objectProvider")]
        public string ObjectProviderName { get; set; }

        [JsonProperty("getExecutionStatusUrl")]
        public string GetExecutionStatusUrl { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("expirationDateTimeUtc")]
        public DateTime ExpirationDateTimeUtc { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, string> ExecutorProperties = new Dictionary<string, string>();

        [JsonProperty("executionParameters")]
        public JObject ExecutionSettings { get; set; }

        [JsonProperty("services")]
        public JObject Services { get; set; }
    }
}
