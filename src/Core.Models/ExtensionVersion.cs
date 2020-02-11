using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Draco.Core.Models
{
    public class ExtensionVersion
    {
        public string ExtensionId { get; set; }
        public string ExtensionVersionId { get; set; }
        public string ReleaseNotes { get; set; }
        public string RequestTypeName { get; set; }
        public string RequestTypeUrl { get; set; }
        public string ResponseTypeName { get; set; }
        public string ResponseTypeUrl { get; set; }

        public bool IsActive { get; set; }
        public bool IsLongRunning { get; set; }
        public bool SupportsValidation { get; set; }

        public List<ExecutionProfile> ExecutionProfiles { get; set; } = new List<ExecutionProfile>();

        public List<ExtensionInputObject> InputObjects { get; set; } = new List<ExtensionInputObject>();

        public List<ExtensionOutputObject> OutputObjects { get; set; } = new List<ExtensionOutputObject>();

        public Dictionary<string, JObject> SupportedServices { get; set; } = new Dictionary<string, JObject>();

        public TimeSpan? ExecutionExpirationPeriod { get; set; }

        public string Version { get; set; }

        public override string ToString() => $"{ExtensionId}_{ExtensionVersionId}";
    }
}
