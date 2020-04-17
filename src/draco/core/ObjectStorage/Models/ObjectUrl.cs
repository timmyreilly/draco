// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Draco.Core.ObjectStorage.Models
{
    /// <summary>
    /// URL + associated metadata used to access an execution object.
    /// For URL-based object accessors, this model is typically the accessor itself.
    /// For more information on execution objects and accessors, see /doc/architecture/execution-objects.md.
    /// </summary>
    public class ObjectUrl
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("urlSignature", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Signature { get; set; }

        [JsonProperty("httpMethod")]
        public string HttpMethod { get; set; }

        [JsonProperty("accessMode")]
        public string AccessMode { get; set; }

        [JsonProperty("expirationDateTimeUtc", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? ExpirationDateTimeUtc { get; set; }

        [JsonProperty("requestHeaders", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, string> RequestHeaders { get; set; }
    }
}
