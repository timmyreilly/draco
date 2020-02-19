// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Draco.ExtensionManagement.Api.Models
{
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
