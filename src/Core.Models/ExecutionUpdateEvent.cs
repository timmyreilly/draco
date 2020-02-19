// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models.Enumerations;
using System;
using System.Collections.Generic;

namespace Draco.Core.Models
{
    public class ExecutionUpdateEvent
    {
        public string ExecutionId { get; set; }
        public string ExtensionId { get; set; }
        public string ExtensionVersionId { get; set; }
        public string StatusMessage { get; set; }

        public double? PercentageComplete { get; set; }

        public DateTime UpdateDateTimeUtc { get; set; }

        public Dictionary<string, string> ExecutorProperties { get; set; } = new Dictionary<string, string>();

        public ExecutionPriority Priority { get; set; }

        public ExecutionStatus Status { get; set; }

        public ExecutorContext Executor { get; set; }
    }
}
