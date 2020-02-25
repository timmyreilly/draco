// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models.Enumerations;
using Newtonsoft.Json.Linq;
using System;

namespace Draco.Core.Models
{
    public class ExecutionProfile
    {
        public string ProfileName { get; set; }
        public string ProfileDescription { get; set; }
        public string ExecutionModelName { get; set; }
        public string ObjectProviderName { get; set; }

        public bool IsActive { get; set; }

        public TimeSpan? DirectExecutionTokenDuration { get; set; }

        public double? BaseExecutionCost { get; set; }

        public ExecutionMode ExecutionMode { get; set; }

        public ExecutionPriority SupportedPriorities { get; set; }

        public JObject ClientConfiguration { get; set; }
        public JObject ExtensionSettings { get; set; }
    }
}
