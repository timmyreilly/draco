// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Draco.ExtensionManagement.Api.Models
{
    public class InputObjectApiModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("extensionId")]
        public string ExtensionId { get; set; }

        [JsonProperty("extensionVersionId")]
        public string ExtensionVersionId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("objectTypeName")]
        public string ObjectTypeName { get; set; }

        [JsonProperty("objectTypeUrl")]
        public string ObjectTypeUrl { get; set; }

        [JsonProperty("isRequired")]
        public bool IsRequired { get; set; }
    }
}
