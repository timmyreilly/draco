// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Draco.Core.Execution.Interfaces
{
    public interface IExecutionProcessorOptions
    {
        TimeSpan DefaultExecutionTimeoutDuration { get; }
    }
}
