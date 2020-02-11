using Newtonsoft.Json;
using System.Collections.Generic;

namespace Draco.Execution.Api.Models
{
    public class ContinueExecutionApiModel
    {
        [JsonProperty("providedObjects")]
        public List<string> ProvidedInputObjects { get; set; } = new List<string>();
    }
}
