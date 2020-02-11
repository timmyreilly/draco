using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Draco.Catalog.Api.Models
{
    public class ExecutionProfileApiModel
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("executionMode")]
        public string ExecutionMode { get; set; }

        [JsonProperty("supportedPriorities")]
        public List<string> SupportedPriorities { get; set; } = new List<string>();

        [JsonProperty("clientConfiguration")]
        public JObject ClientConfiguration { get; set; }
    }
}
