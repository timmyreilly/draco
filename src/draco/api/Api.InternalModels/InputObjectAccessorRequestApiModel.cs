// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System;

namespace Draco.Api.InternalModels
{
    public class InputObjectAccessorRequestApiModel
    {
        [JsonProperty("objectName")]
        public string ObjectName { get; set; }

        [JsonProperty("objectProviderName")]
        public string ObjectProviderName { get; set; }

        [JsonProperty("signatureRsaKeyXml")]
        public string SignatureRsaKeyXml { get; set; }

        [JsonProperty("expirationPeriod")]
        public TimeSpan? ExpirationPeriod { get; set; }

        [JsonProperty("objectMetadata")]
        public InputObjectApiModel ObjectMetadata { get; set; }

        [JsonProperty("executionMetadata")]
        public ExecutionMetadataApiModel ExecutionMetadata { get; set; }
    }
}
