// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Draco.Execution.Api.Models
{
    public class ExecutionUpdateApiModel
    {
        [JsonProperty("executionId")]
        public string ExecutionId { get; set; }

        [JsonProperty("status")]
        public string ExecutionStatus { get; set; }

        [JsonProperty("statusMessage", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string StatusMessage { get; set; }

        [JsonProperty("getExecutionStatusUrl")]
        public string GetExecutionStatusUrl { get; set; }

        [JsonProperty("putExecutionStatusUrl")]
        public string PutExecutionStatusUrl { get; set; }

        [JsonProperty("percentComplete", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public double? PercentComplete { get; set; }

        [JsonProperty("lastUpdatedUtc")]
        public DateTime LastUpdatedDateTimeUtc { get; set; }

        [JsonProperty("expirationDateTimeUtc", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime ExpirationDateTimeUtc { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, string> ExecutorProperties = new Dictionary<string, string>();

        [JsonProperty("resultData", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JObject ResultData { get; set; }

        [JsonProperty("inputObjects", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, InputObjectApiModel> InputObjects { get; set; }

        [JsonProperty("outputObjects", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, OutputObjectApiModel> OutputObjects { get; set; }

        [JsonProperty("validationErrors", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ValidationErrorApiModel> ValidationErrors { get; set; }
    }
}
