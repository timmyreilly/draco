// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Draco.Azure.Models.Cosmos
{
    public class ExtensionVersion
    {
        [JsonProperty("id")]
        public string Id => $"{ExtensionId}_{ExtensionVersionId}";

        [JsonProperty("modelType")]
        public string ModelType => ModelTypes.V1.ExtensionVersion;

        [JsonProperty("extensionId")]
        public string ExtensionId { get; set; }

        [JsonProperty("extensionVersionId")]
        public string ExtensionVersionId { get; set; }

        [JsonProperty("releaseNotes")]
        public string ReleaseNotes { get; set; }

        [JsonProperty("requestTypeName")]
        public string RequestTypeName { get; set; }

        [JsonProperty("requestTypeUrl")]
        public string RequestTypeUrl { get; set; }

        [JsonProperty("responseTypeName")]
        public string ResponseTypeName { get; set; }

        [JsonProperty("responseTypeUrl")]
        public string ResponseTypeUrl { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("isLongRunning")]
        public bool IsLongRunning { get; set; }

        [JsonProperty("supportsValidation")]
        public bool SupportsValidation { get; set; }

        [JsonProperty("executionProfiles")]
        public Dictionary<string, ExecutionProfile> ExecutionProfiles { get; set; } = new Dictionary<string, ExecutionProfile>();

        [JsonProperty("inputObjects")]
        public List<ExtensionInputObject> InputObjects { get; set; } = new List<ExtensionInputObject>();

        [JsonProperty("outputObjects")]
        public List<ExtensionOutputObject> OutputObjects { get; set; } = new List<ExtensionOutputObject>();

        [JsonProperty("supportedServices")]
        public Dictionary<string, JObject> SupportedServices { get; set; } = new Dictionary<string, JObject>();

        [JsonProperty("executionExpirationPeriod")]
        public TimeSpan? ExecutionExpirationPeriod { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}
