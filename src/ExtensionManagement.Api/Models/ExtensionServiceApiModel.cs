// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Draco.ExtensionManagement.Api.Models
{
    public class ExtensionServiceApiModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("extensionId")]
        public string ExtensionId { get; set; }

        [JsonProperty("extensionVersionId")]
        public string ExtensionVersionId { get; set; }

        [JsonProperty("configuration")]
        public Dictionary<string, string> ServiceConfiguration { get; set; } = new Dictionary<string, string>();
    }
}
