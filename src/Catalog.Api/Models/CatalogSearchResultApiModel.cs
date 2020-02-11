using Newtonsoft.Json;
using System.Collections.Generic;

namespace Draco.Catalog.Api.Models
{
    public class CatalogSearchResultApiModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("publisherName")]
        public string PublisherName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("subcategory")]
        public string Subcategory { get; set; }

        [JsonProperty("getExtensionDetailUrl")]
        public string ExtensionDetailUrl { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
    }
}
