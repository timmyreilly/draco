// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Draco.Execution.Api.Models
{
    public class ToContinueExecutionApiModel
    {
        [JsonProperty("executionId")]
        public string ExecutionId { get; set; }

        [JsonProperty("executionStatus")]
        public string ExecutionStatus { get; set; }

        [JsonProperty("putContinueExecutionUrl")]
        public string PutContinueExecutionUrl { get; set; }

        [JsonProperty("getExecutionStatusUrl")]
        public string GetExecutionStatusUrl { get; set; }

        [JsonProperty("message", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty("properties")]
        public Dictionary<string, string> ExecutorProperties = new Dictionary<string, string>();

        [JsonProperty("provideObjects")]
        public Dictionary<string, InputObjectApiModel> InputObjects { get; set; } = new Dictionary<string, InputObjectApiModel>();
    }
}
