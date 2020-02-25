// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models.Enumerations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Draco.Api.InternalModels
{
    public class ExecutionRequestApiModel
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

        [JsonProperty("statusUpdateKey")]
        public string StatusUpdateKey { get; set; }

        [JsonProperty("getExecutionStatusUrl")]
        public string GetExecutionStatusUrl { get; set; }

        [JsonProperty("putExecutionStatusUrl")]
        public string PutExecutionStatusUrl { get; set; }

        [JsonProperty("signatureRsaKeyXml")]
        public string SignatureRsaKeyXml { get; set; }

        [JsonProperty("isValidationSupported")]
        public bool IsValidationSupported { get; set; }

        [JsonProperty("validateOnly")]
        public bool ValidateOnly { get; set; }

        [JsonProperty("createdDateTimeUtc")]
        public DateTime CreatedDateTimeUtc { get; set; }

        [JsonProperty("lastUpdatedDateTimeUtc")]
        public DateTime LastUpdatedDateTimeUtc { get; set; }

        [JsonProperty("executionTimeoutDateTimeUtc")]
        public DateTime? ExecutionTimeoutDateTimeUtc { get; set; }

        [JsonProperty("executionTimeoutDuration")]
        public TimeSpan? ExecutionTimeoutDuration { get; set; }

        [JsonProperty("executor")]
        public ExecutorContextApiModel Executor { get; set; }

        [JsonProperty("executionParameters")]
        public JObject ExecutionParameters { get; set; }

        [JsonProperty("extensionSettings")]
        public JObject ExtensionSettings { get; set; }

        [JsonProperty("executorProperties")]
        public Dictionary<string, string> ExecutorProperties { get; set; } = new Dictionary<string, string>();

        [JsonProperty("inputObjects")]
        public Dictionary<string, InputObjectApiModel> InputObjects { get; set; } = new Dictionary<string, InputObjectApiModel>();

        [JsonProperty("outputObjects")]
        public Dictionary<string, OutputObjectApiModel> OutputObjects { get; set; } = new Dictionary<string, OutputObjectApiModel>();

        [JsonProperty("supportedServices")]
        public Dictionary<string, JObject> SupportedServices { get; set; } = new Dictionary<string, JObject>();

        [JsonProperty("priority")]
        public ExecutionPriority Priority { get; set; }
    }
}
