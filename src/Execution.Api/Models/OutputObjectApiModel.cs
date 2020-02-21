// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Draco.Execution.Api.Models
{
    public class OutputObjectApiModel
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("objectProvider")]
        public string ObjectProviderName { get; set; }

        [JsonProperty("objectTypeName")]
        public string ObjectTypeName { get; set; }

        [JsonProperty("objectTypeUrl")]
        public string ObjectTypeUrl { get; set; }

        [JsonProperty("toDownload")]
        public JObject Accessor { get; set; }
    }
}
