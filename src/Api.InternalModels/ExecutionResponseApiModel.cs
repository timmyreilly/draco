using Draco.Core.Models.Enumerations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Draco.Api.InternalModels
{
    public class ExecutionResponseApiModel
    {
        [JsonProperty("executionId")]
        public string ExecutionId { get; set; }

        [JsonProperty("extensionId")]
        public string ExtensionId { get; set; }

        [JsonProperty("extensionVersionId")]
        public string ExtensionVersionId { get; set; }

        [JsonProperty("executionModelName")]
        public string ExecutionModelName { get; set; }

        [JsonProperty("executionProfileName")]
        public string ExecutionProfileName { get; set; }

        [JsonProperty("objectProviderName")]
        public string ObjectProviderName { get; set; }

        [JsonProperty("statusMessage")]
        public string StatusMessage { get; set; }

        [JsonProperty("statusUpdateKey")]
        public string StatusUpdateKey { get; set; }

        [JsonProperty("percentComplete")]
        public double? PercentComplete { get; set; }

        [JsonProperty("createdDateTimeUtc")]
        public DateTime CreatedDateTimeUtc { get; set; }

        [JsonProperty("lastUpdatedDateTimeUtc")]
        public DateTime LastUpdatedDateTimeUtc { get; set; }

        [JsonProperty("executionTimeoutDateTimeUtc")]
        public DateTime? ExecutionTimeoutDateTimeUtc { get; set; }

        [JsonProperty("inputObjects")]
        public Dictionary<string, InputObjectApiModel> InputObjects { get; set; } = new Dictionary<string, InputObjectApiModel>();

        [JsonProperty("outputObjects")]
        public Dictionary<string, OutputObjectApiModel> OutputObjects { get; set; } = new Dictionary<string, OutputObjectApiModel>();

        [JsonProperty("executor")]
        public ExecutorContextApiModel Executor { get; set; }

        [JsonProperty("priority")]
        public ExecutionPriority Priority { get; set; }

        [JsonProperty("status")]
        public ExecutionStatus Status { get; set; }

        [JsonProperty("resultData")]
        public JObject ResultData { get; set; }

        [JsonProperty("executorProperties")]
        public Dictionary<string, string> ExecutorProperties { get; set; } = new Dictionary<string, string>();

        [JsonProperty("validationErrors")]
        public List<ValidationErrorApiModel> ValidationErrors { get; set; } = new List<ValidationErrorApiModel>();

        [JsonProperty("supportedServices")]
        public Dictionary<string, JObject> SupportedServices { get; set; } = new Dictionary<string, JObject>();
    }
}
