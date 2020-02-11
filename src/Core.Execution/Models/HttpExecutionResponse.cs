using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Draco.Core.Execution.Models
{
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
