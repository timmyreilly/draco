using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Draco.Core.Models
{
    public class ExecutionUpdate
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("statusMessage")]
        public string StatusMessage { get; set; }

        [JsonProperty("statusUpdateKey")]
        public string StatusUpdateKey { get; set; }

        [JsonProperty("percentComplete")]
        public double? PercentComplete { get; set; }

        [JsonProperty("lastUpdatedDateTimeUtc")]
        public DateTime? LastUpdatedDateTimeUtc { get; set; }

        [JsonProperty("executionTimeoutDateTimeUtc")]
        public DateTime? ExecutionTimeoutDateTimeUtc { get; set; }

        [JsonProperty("resultData")]
        public JObject ResultData { get; set; }

        [JsonProperty("validationErrors")]
        public List<ExecutionValidationError> ValidationErrors { get; set; } = new List<ExecutionValidationError>();

        [JsonProperty("properties")]
        public Dictionary<string, string> ExecutorProperties { get; set; } = new Dictionary<string, string>();

        [JsonProperty("providedOutputObjects")]
        public List<string> ProvidedOutputObjects { get; set; } = new List<string>();
    }
}
