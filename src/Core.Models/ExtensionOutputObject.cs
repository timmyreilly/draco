using Newtonsoft.Json;

namespace Draco.Core.Models
{
    public class ExtensionOutputObject
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("objectTypeName")]
        public string ObjectTypeName { get; set; }

        [JsonProperty("objectTypeUrl")]
        public string ObjectTypeUrl { get; set; }
    }
}
