// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Draco.Core.Execution.Interfaces
{
    /// <summary>
    /// Defines common, generic options used by execution processors.
    /// </summary>
    public interface IExecutionProcessorOptions
    {
        TimeSpan DefaultExecutionTimeoutDuration { get; }
    }
}
