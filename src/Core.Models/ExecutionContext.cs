// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models.Enumerations;
using Draco.Core.Models.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Draco.Core.Models
{
    public class ExecutionContext : IExecutionMetadata
    {
        public string ExecutionId { get; set; }
        public string ExtensionId { get; set; }
        public string ExtensionVersionId { get; set; }
        public string ExecutionModelName { get; set; }
        public string ExecutionProfileName { get; set; }
        public string ObjectProviderName { get; set; }
        public string StatusMessage { get; set; }
        public string StatusUpdateKey { get; set; }

        public double? PercentComplete { get; set; }

        public DateTime CreatedDateTimeUtc { get; set; }
        public DateTime LastUpdatedDateTimeUtc { get; set; }

        public DateTime? ExecutionTimeoutDateTimeUtc { get; set; }

        public Dictionary<string, ExtensionInputObject> InputObjects { get; set; } = new Dictionary<string, ExtensionInputObject>();

        public Dictionary<string, ExtensionOutputObject> OutputObjects { get; set; } = new Dictionary<string, ExtensionOutputObject>();

        public List<string> ProvidedInputObjects { get; set; } = new List<string>();
        public List<string> ProvidedOutputObjects { get; set; } = new List<string>();

        public ExecutorContext Executor { get; set; }

        public ExecutionPriority Priority { get; set; }

        public ExecutionStatus Status { get; set; }

        public JObject ResultData { get; set; }

        public Dictionary<string, string> ExecutorProperties { get; set; } = new Dictionary<string, string>();

        public List<ExecutionValidationError> ValidationErrors { get; set; } = new List<ExecutionValidationError>();

        public Dictionary<string, JObject> SupportedServices { get; set; } = new Dictionary<string, JObject>();
    }
}
