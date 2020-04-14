// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Draco.Core.Execution.Models
{
    /// <summary>
    /// This is the execution response model expected from the target extension when using the 
    /// "http-json/async/v1" or "http-json/sync/v1" execution models. For more information on execution models,
    /// see /doc/architecture/execution-models.md.
    /// </summary>
    public class HttpExecutionResponse
    {
        [JsonProperty("executionId")]
        public string ExecutionId { get; set; }
        
        [JsonProperty("responseData")]
        public JObject ResponseData { get; set; }

        [JsonProperty("providedOutputObjects")]
        public List<string> ProvidedOutputObjects { get; set; } = new List<string>();

        [JsonProperty("validationErrors")]
        public List<HttpExecutionValidationError> ValidationErrors { get; set; } = new List<HttpExecutionValidationError>();
    }
}
