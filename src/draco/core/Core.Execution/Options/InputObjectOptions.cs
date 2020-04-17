// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Interfaces;
using System;

namespace Draco.Core.Execution.Options
{
    /// <summary>
    /// Common options used to create input object accessors.
    /// For more information on input objects, see /doc/architecture/execution-objects.md.
    /// </summary>
    public class InputObjectOptions : IInputObjectOptions
    {
        public TimeSpan DefaultTimeoutDuration { get; set; } = TimeSpan.FromHours(1);
    }

    /// <summary>
    /// Provider-specific options used to create input object accessors.
    /// For more information on input objecs, see /doc/architecture/execution-objects.md.
    /// </summary>
    /// <typeparam name="T">The target input object accessor provider type</typeparam>
    public class InputObjectOptions<T> : InputObjectOptions { }
}
