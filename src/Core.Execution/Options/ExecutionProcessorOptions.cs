// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Interfaces;
using System;

namespace Draco.Core.Execution.Options
{
    public class ExecutionProcessorOptions<T> : IExecutionProcessorOptions
    {
        public TimeSpan DefaultExecutionTimeoutDuration { get; set; } = TimeSpan.FromHours(1);
    }
}
