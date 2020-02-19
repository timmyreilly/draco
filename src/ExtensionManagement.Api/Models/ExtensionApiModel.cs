// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Draco.ExtensionManagement.Api.Models
{
    public class ExtensionApiModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("subcategory")]
        public string Subcategory { get; set; }

        [JsonProperty("coverImageUrl")]
        public string ExtensionCoverImageUrl { get; set; }

        [JsonProperty("logoUrl")]
        public string ExtensionLogoUrl { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("publisherName")]
        public string PublisherName { get; set; }

        [JsonProperty("copyrightNotice")]
        public string CopyrightNotice { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("additionalInformationUrls")]
        public Dictionary<string, string> AdditionalInformationUrls { get; set; } = new Dictionary<string, string>();

        [JsonProperty("features")]
        public Dictionary<string, string> Features { get; set; } = new Dictionary<string, string>();

        [JsonProperty("tags")]
        public List<string> Tags = new List<string>();

    }
}
