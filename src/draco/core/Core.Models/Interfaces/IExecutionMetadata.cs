// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models.Enumerations;

namespace Draco.Core.Models.Interfaces
{
    public interface IExecutionMetadata
    {
        string ExecutionId { get; set; }
        string ExtensionId { get; set; }
        string ExtensionVersionId { get; set; }
        string ExecutionProfileName { get; set; }

        ExecutorContext Executor { get; set; }

        ExecutionPriority Priority { get; set; }
    }
}