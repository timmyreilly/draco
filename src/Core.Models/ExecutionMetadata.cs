// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models.Enumerations;
using Draco.Core.Models.Interfaces;

namespace Draco.Core.Models
{
    public class ExecutionMetadata : IExecutionMetadata
    {
        public string ExecutionId { get; set; }
        public string ExtensionId { get; set; }
        public string ExtensionVersionId { get; set; }
        public string ExecutionProfileName { get; set; }

        public ExecutorContext Executor { get; set; }

        public ExecutionPriority Priority { get; set; }
    }
}
