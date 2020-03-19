// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Draco.Api.InternalModels
{
    /// <summary>
    /// Internal API model for core input object definition -- /src/draco/core/Core.Models/ExtensionInputObject.cs
    /// </summary>
    public class InputObjectApiModel
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("objectTypeName")]
        public string ObjectTypeName { get; set; }

        [JsonProperty("objectTypeUrl")]
        public string ObjectTypeUrl { get; set; }

        [JsonProperty("isProvided")]
        public bool IsProvided { get; set; }

        [JsonProperty("isRequired")]
        public bool IsRequired { get; set; }
    }
}
