// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System;

namespace Draco.Api.InternalModels
{
    /// <summary>
    /// Internal API model for core output object accessor request -- /src/draco/core/ObjectStorage/Models/OutputObjectAccessorRequest.cs
    /// </summary>
    public class OutputObjectAccessorRequestApiModel
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
        public OutputObjectApiModel ObjectMetadata { get; set; }

        [JsonProperty("executionMetadata")]
        public ExecutionMetadataApiModel ExecutionMetadata { get; set; }
    }
}
