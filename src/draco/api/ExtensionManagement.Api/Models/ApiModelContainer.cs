// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Draco.ExtensionManagement.Api.Models
{
    /// <summary>
    /// Every extension management API action that returns data does so using this API container model.
    /// This container model defines both the appropriate data [model] and appropriate API links [links] for next steps.
    /// TODO: Add a feature flag that toggles on/off [links].
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiModelContainer<T>
    {
        public ApiModelContainer() { }

        public ApiModelContainer(T model)
        {
            Model = model;
        }

        [JsonProperty("model")]
        public T Model { get; set; }

        [JsonProperty("links")]
        public Dictionary<string, string> Links = new Dictionary<string, string>();
    }
}
