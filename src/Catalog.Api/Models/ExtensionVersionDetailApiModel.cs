// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Draco.Catalog.Api.Models
{
    public class ExtensionVersionDetailApiModel
    {
        [JsonProperty("id")]
        public string VersionId { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("releaseNotes")]
        public string ReleaseNotes { get; set; }

        [JsonProperty("requestTypeName")]
        public string RequestTypeName { get; set; }

        [JsonProperty("responseTypeName")]
        public string ResponseTypeName { get; set; }

        [JsonProperty("getRequestTypeUrl")]
        public string RequestTypeUrl { get; set; }

        [JsonProperty("getResponseTypeUrl")]
        public string ResponseTypeUrl { get; set; }

        [JsonProperty("getExtensionDetailUrl")]
        public string ExtensionDetailUrl { get; set; }

        [JsonProperty("isLongRunning")]
        public bool IsLongRunning { get; set; }

        [JsonProperty("supportsValidation")]
        public bool SupportsValidation { get; set; }

        [JsonProperty("executionProfiles")]
        public Dictionary<string, ExecutionProfileApiModel> ExecutionProfiles = new Dictionary<string, ExecutionProfileApiModel>();

        [JsonProperty("inputObjects")]
        public Dictionary<string, InputObjectApiModel> InputObjects = new Dictionary<string, InputObjectApiModel>();

        [JsonProperty("outputObjects")]
        public Dictionary<string, OutputObjectApiModel> OutputObjects = new Dictionary<string, OutputObjectApiModel>();
    }
}
