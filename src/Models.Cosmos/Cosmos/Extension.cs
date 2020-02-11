using System.Collections.Generic;
using Newtonsoft.Json;

namespace Draco.Azure.Models.Cosmos
{
    public class Extension
    {
        [JsonProperty("id")]
        public string Id => ExtensionId;

        [JsonProperty("modelType")]
        public string ModelType => ModelTypes.V1.Extension;    

        [JsonProperty("extensionId")]
        public string ExtensionId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("subcategory")]
        public string Subcategory { get; set; }

        [JsonProperty("publisherName")]
        public string PublisherName { get; set; }

        [JsonProperty("copyrightNotice")]
        public string CopyrightNotice { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("extensionLogoUrl")]
        public string ExtensionLogoUrl { get; set; }

        [JsonProperty("extensionCoverImageUrl")]
        public string ExtensionCoverImageUrl { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("additionalInformationUrls")]
        public Dictionary<string, string> AdditionalInformationUrls { get; set; } = new Dictionary<string, string>();

        [JsonProperty("features")]
        public Dictionary<string, string> Features { get; set; } = new Dictionary<string, string>();

        [JsonProperty("tags")]
        public List<string> Tags { get; set; } = new List<string>();
    }
}
