using Newtonsoft.Json;

namespace Draco.Catalog.Api.Models
{
    public class ExtensionVersionListItemModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("releaseNotes")]
        public string ReleaseNotes { get; set; }

        [JsonProperty("getVersionDetailUrl")]
        public string DetailUrl { get; set; }

        [JsonProperty("isCurrent")]
        public bool IsCurrent { get; set; }
    }
}
