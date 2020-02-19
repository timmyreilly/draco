// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Draco.Catalog.Api.Models
{
    public class ExtensionDetailApiModel
    {
        [JsonProperty("id")]
        public string ExtensionId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("publisherName")]
        public string PublisherName { get; set; }

        [JsonProperty("copyrightNotice")]
        public string CopyrightNotice { get; set; }

        [JsonProperty("extensionLogoUrl")]
        public string ExtensionLogoUrl { get; set; }

        [JsonProperty("extensionCoverImageUrl")]
        public string ExtensionCoverImageUrl { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("subcategory")]
        public string Subcategory { get; set; }

        [JsonProperty("additionalInformationUrls")]
        public Dictionary<string, string> AdditionalInformationUrls { get; set; } = new Dictionary<string, string>();

        [JsonProperty("tags")]
        public List<string> Tags { get; set; } = new List<string>();

        [JsonProperty("clientRequirements")]
        public List<string> ClientRequirements { get; set; } = new List<string>();

        [JsonProperty("availableVersions")]
        public List<ExtensionVersionListItemModel> ExtensionVersions { get; set; } = new List<ExtensionVersionListItemModel>();
    }
}
