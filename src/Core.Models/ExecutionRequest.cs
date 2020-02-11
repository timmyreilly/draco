using Draco.Core.Models.Enumerations;
using Draco.Core.Models.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Draco.Core.Models
{
    public class ExecutionRequest : IExecutionMetadata
    {
        public string ExecutionId { get; set; } 
        public string ExtensionId { get; set; }
        public string ExecutionModelName { get; set; }
        public string ExecutionProfileName { get; set; }
        public string ObjectProviderName { get; set; }
        public string StatusUpdateKey { get; set; }
        public string GetExecutionStatusUrl { get; set; }
        public string UpdateExecutionStatusUrl { get; set; }
        public string SignatureRsaKeyXml { get; set; }

        public bool IsValidationSupported { get; set; }
        public bool ValidateOnly { get; set; }

        public DateTime CreatedDateTimeUtc { get; set; }
        public DateTime LastUpdatedDateTimeUtc { get; set; }

        public DateTime? ExecutionTimeoutDateTimeUtc { get; set; }

        public TimeSpan? ExecutionTimeoutDuration { get; set; }

        public ExecutorContext Executor { get; set; }

        public JObject ExecutionParameters { get; set; }
        public JObject ExtensionSettings { get; set; }

        public string ExtensionVersionId { get; set; }

        public Dictionary<string, string> ExecutorProperties { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, ExtensionInputObject> InputObjects { get; set; }
        
        public Dictionary<string, ExtensionOutputObject> OutputObjects { get; set; }

        public List<string> ProvidedInputObjects { get; set; } = new List<string>();

        public Dictionary<string, JObject> SupportedServices { get; set; } = new Dictionary<string, JObject>();

        public ExecutionPriority Priority { get; set; }
    }
}
