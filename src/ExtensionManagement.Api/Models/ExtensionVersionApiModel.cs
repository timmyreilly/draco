// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System;

namespace Draco.ExtensionManagement.Api.Models
{
    public class ExtensionVersionApiModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("extensionId")]
        public string ExtensionId { get; set; }

        [JsonProperty("releaseNotes")]
        public string ReleaseNotes { get; set; }

        [JsonProperty("requestTypeName")]
        public string RequestTypeName { get; set; }

        [JsonProperty("responseTypeName")]
        public string ResponseTypeName { get; set; }

        [JsonProperty("requestTypeUrl")]
        public string RequestTypeUrl { get; set; }

        [JsonProperty("responseTypeUrl")]
        public string ResponseTypeUrl { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("isLongRunning")]
        public bool IsLongRunning { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("supportsValidation")]
        public bool SupportsValidation { get; set; }

        [JsonProperty("executionExpirationPeriod")]
        public TimeSpan? ExecutionExpirationPeriod { get; set; }
    }
}
