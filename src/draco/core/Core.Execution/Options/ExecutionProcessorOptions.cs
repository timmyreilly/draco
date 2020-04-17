// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Interfaces;
using System;

namespace Draco.Core.Execution.Options
{
    /// <summary>
    /// Common options used to configure execution processors.
    /// For more information on execution processors and the role that they play in the Draco execution pipeline,
    /// see /doc/architecture/execution-pipeline.md.
    /// </summary>
    /// <typeparam name="T">The configured execution processor type</typeparam>
    public class ExecutionProcessorOptions<T> : IExecutionProcessorOptions
    {
        public TimeSpan DefaultExecutionTimeoutDuration { get; set; } = TimeSpan.FromHours(1);
    }
}
