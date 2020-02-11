using Draco.Core.Models.Enumerations;
using Newtonsoft.Json.Linq;

namespace Draco.Core.Models
{
    public class ClientExecutionRequest
    {
        public string ExtensionId { get; set; }
        public string ExtensionVersionId { get; set; }
        public string ExecutionProfileName { get; set; }

        public bool ValidateOnly { get; set; }

        public JObject ExecutionRequest { get; set; }

        public ExecutionPriority Priority { get; set; }
    }
}
