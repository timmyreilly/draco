// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Draco.Core.Models
{
    public class InputObjectAccessor
    {
        public InputObjectAccessor() { }

        public InputObjectAccessor(ExtensionInputObject objectMetadata, JObject objectAccessor)
        {
            ObjectMetadata = objectMetadata;
            ObjectAccessor = objectAccessor;
        }

        [JsonProperty("metadata")]
        public ExtensionInputObject ObjectMetadata { get; set; }

        [JsonProperty("accessor")]
        public JObject ObjectAccessor { get; set; }
    }
}
