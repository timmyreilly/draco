// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Draco.Core.Execution.Interfaces
{
    /// <summary>
    /// Defines common options for creating output object accessors.
    /// For more information on output objects, see /doc/architecture/execution-objects.md.
    /// </summary>
    public interface IOutputObjectOptions
    {
        /// <summary>
        /// Gets the default period that an output object accessor should be valid for.
        /// </summary>
        /// <value>The default period that an output object accessor should be valid for</value>
        TimeSpan DefaultTimeoutDuration { get; }
    }
}
