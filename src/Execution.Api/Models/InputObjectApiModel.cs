using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Draco.Execution.Api.Models
{
    public class InputObjectApiModel
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("objectProvider")]
        public string ObjectProviderName { get; set; }

        [JsonProperty("objectTypeName")]
        public string ObjectTypeName { get; set; }

        [JsonProperty("objectTypeUrl")]
        public string ObjectTypeUrl { get; set; }

        [JsonProperty("isRequired")]
        public bool IsRequired { get; set; }

        [JsonProperty("toUpload")]
        public JObject Accessor { get; set; }
    }
}
