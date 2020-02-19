// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.Core.Models.Enumerations;
using Draco.Core.Models.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Draco.Azure.Models.Cosmos
{
    public class Execution : IExecutionMetadata
    {
        [JsonProperty("id")]
        public string Id => ExecutionId;

        [JsonProperty("modelType")]
        public string ModelType => ModelTypes.V1.Execution;

        [JsonProperty("executionId")]
        public string ExecutionId { get; set; }

        [JsonProperty("executionModel")]
        public string ExecutionModelName { get; set; }

        [JsonProperty("executionProfile")]
        public string ExecutionProfileName { get; set; }

        [JsonProperty("objectProvider")]
        public string ObjectProviderName { get; set; }

        [JsonProperty("extensionId")]
        public string ExtensionId { get; set; }

        [JsonProperty("extensionVersionId")]
        public string ExtensionVersionId { get; set; }

        [JsonProperty("signatureRsaKeyXml")]
        public string SignatureRsaKeyXml { get; set; }

        [JsonProperty("statusMessage")]
        public string StatusMessage { get; set; }

        [JsonProperty("statusUpdateKey")]
        public string StatusUpdateKey { get; set; }

        [JsonProperty("validateOnly")]
        public bool ValidateOnly { get; set; }

        [JsonProperty("createdDateTimeUtc")]
        public DateTime CreatedDateTimeUtc { get; set; }

        [JsonProperty("lastUpdatedDateTimeUtc")]
        public DateTime LastUpdatedDateTimeUtc { get; set; }

        [JsonProperty("expiresDateTimeUtc")]
        public DateTime ExpiresDateTimeUtc { get; set; }

        [JsonProperty("pctComplete")]
        public double? PercentageComplete { get; set; }

        [JsonProperty("mode")]
        public ExecutionMode Mode { get; set; }

        [JsonProperty("priority")]
        public ExecutionPriority Priority { get; set; }

        [JsonProperty("status")]
        public ExecutionStatus Status { get; set; }

        [JsonProperty("executor")]
        public ExecutorContext Executor { get; set; }

        [JsonProperty("requestData")]
        public JObject RequestData { get; set; }

        [JsonProperty("resultData")]
        public JObject ResultData { get; set; }

        [JsonProperty("executorProperties")]
        public Dictionary<string, string> ExecutorProperties { get; set; } = new Dictionary<string, string>();

        [JsonProperty("inputObjects")]
        public List<ExtensionInputObject> InputObjects { get; set; } = new List<ExtensionInputObject>();

        [JsonProperty("outputObjects")]
        public List<ExtensionOutputObject> OutputObjects { get; set; } = new List<ExtensionOutputObject>();

        [JsonProperty("providedInputObjects")]
        public List<string> ProvidedInputObjects = new List<string>();

        [JsonProperty("providedOutputObjects")]
        public List<string> ProvidedOutputObjects = new List<string>();
    }
}
