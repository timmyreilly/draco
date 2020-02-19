// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Draco.Catalog.Api.Models
{
    public class InputObjectApiModel
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("objectTypeName")]
        public string ObjectTypeName { get; set; }

        [JsonProperty("objectTypeUrl")]
        public string ObjectTypeUrl { get; set; }

        [JsonProperty("isRequired")]
        public bool IsRequired { get; set; }
    }
}
