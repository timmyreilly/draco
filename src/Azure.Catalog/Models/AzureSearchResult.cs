// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Draco.Azure.Catalog.Models
{
    public class AzureSearchResult
    {
        [JsonProperty("extensionId")]
        public string ExtensionId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("publisherName")]
        public string PublisherName { get; set; }

        [JsonProperty("copyrightNotice")]
        public string CopyrightNotice { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("subcategory")]
        public string Subcategory { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
    }
}
