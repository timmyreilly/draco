// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Draco.Core.Execution.Models
{
    /// <summary>
    /// This is the execution validation error model expected from the target extension when using the 
    /// "http-json/async/v1" or "http-json/sync/v1" execution models. For more information on execution models,
    /// see /doc/architecture/execution-models.md.
    /// </summary>
    public class HttpExecutionValidationError
    {
        [JsonProperty("errorId")]
        public string ErrorId { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("errorData")]
        public JObject ErrorData { get; set; }
    }
}