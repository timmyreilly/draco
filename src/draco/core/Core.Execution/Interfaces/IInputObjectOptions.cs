// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Draco.Core.Execution.Interfaces
{
    /// <summary>
    /// Defines common options for creating input object accessors.
    /// For more information on input objects, see /doc/architecture/execution-objects.md.
    /// </summary>
    public interface IInputObjectOptions
    {
        /// <summary>
        /// Gets the default period that an input object accessor should be valid for.
        /// </summary>
        /// <value>The default period that an input object accessor should be valid for.</value>
        TimeSpan DefaultTimeoutDuration { get; }
    }
}
