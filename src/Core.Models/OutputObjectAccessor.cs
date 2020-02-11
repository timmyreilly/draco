using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Draco.Core.Models
{
    public class OutputObjectAccessor
    {
        public OutputObjectAccessor() { }

        public OutputObjectAccessor(ExtensionOutputObject objectMetadata, JObject objectAccessor)
        {
            ObjectMetadata = objectMetadata;
            ObjectAccessor = objectAccessor;
        }

        [JsonProperty("metadata")]
        public ExtensionOutputObject ObjectMetadata { get; set; }

        [JsonProperty("accessor")]
        public JObject ObjectAccessor { get; set; }
    }
}
