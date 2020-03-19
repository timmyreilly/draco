// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Draco.Api.InternalModels
{
    /// <summary>
    /// Internal API model for core output object definition -- /src/draco/core/Core.Models/ExtensionOutputObject.cs
    /// </summary>
    public class OutputObjectApiModel
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("objectTypeName")]
        public string ObjectTypeName { get; set; }

        [JsonProperty("objectTypeUrl")]
        public string ObjectTypeUrl { get; set; }

        [JsonProperty("isProvided")]
        public bool IsProvided { get; set; }
    }
}
