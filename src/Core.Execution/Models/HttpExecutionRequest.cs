// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.Core.Models.Enumerations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Draco.Core.Execution.Models
{
    public class HttpExecutionRequest
    {
        [JsonProperty("executionId")]
        public string ExecutionId { get; set; }

        [JsonProperty("profileName")]
        public string ExecutionProfileName { get; set; }

        [JsonProperty("extensionId")]
        public string ExtensionId { get; set; }

        [JsonProperty("extensionVersionId")]
        public string ExtensionVersionId { get; set; }

        [JsonProperty("statusUpdateKey")]
        public string StatusUpdateKey { get; set; }

        [JsonProperty("getExecutionStatusUrl")]
        public string GetExecutionStatusUrl { get; set; }

        [JsonProperty("putExecutionStatusUrl")]
        public string UpdateExecutionStatusUrl { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("createdDateTimeUtc")]
        public DateTime CreatedDateTimeUtc { get; set; }

        [JsonProperty("lastUpdatedDateTimeUtc")]
        public DateTime LastUpdatedDateTimeUtc { get; set; }

        [JsonProperty("expirationDateTimeUtc")]
        public DateTime? ExpirationDateTimeUtc { get; set; }

        [JsonProperty("priority")]
        public ExecutionPriority Priority { get; set; }

        [JsonProperty("executor")]
        public ExecutorContext Executor { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, string> ExecutorProperties = new Dictionary<string, string>();

        [JsonProperty("inputObjects")]
        public Dictionary<string, InputObjectAccessor> InputObjects { get; set; } = new Dictionary<string, InputObjectAccessor>();

        [JsonProperty("outputObjects")]
        public Dictionary<string, OutputObjectAccessor> OutputObjects { get; set; } = new Dictionary<string, OutputObjectAccessor>();

        [JsonProperty("requestParameters")]
        public JObject RequestParameters { get; set; }

        [JsonProperty("services")]
        public JObject Services { get; set; }
    }
}
