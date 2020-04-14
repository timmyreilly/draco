// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Draco.Core.Execution.Models
{
    /// <summary>
    /// These are "http-json/async/v1" and "http-json/sync/v1" execution model-specific execution settings 
    /// which are part of the execution request and deserialized at execution-time by the appropriate execution adapter.
    /// </summary>
    public class HttpExtensionSettings
    {
        [JsonProperty("executionUrl")]
        public string ExecutionUrl { get; set; }

        [JsonProperty("validationUrl")]
        public string ValidationUrl { get; set; }
    }
}
