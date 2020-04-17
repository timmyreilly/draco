// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Interfaces;
using System;

namespace Draco.Core.Execution.Options
{
    /// <summary>
    /// Common options used to create output object accessors.
    /// For more information on output objects, see /doc/architecture/execution-objects.md.
    /// </summary>
    public class OutputObjectOptions : IOutputObjectOptions
    {
        public TimeSpan DefaultTimeoutDuration { get; set; } = TimeSpan.FromHours(1);
    }

    /// <summary>
    /// Provider-specific options used to create output object accessors.
    /// For more information on output objecs, see /doc/architecture/execution-objects.md.
    /// </summary>
    /// <typeparam name="T">The target output object accessor provider type</typeparam>
    public class OutputObjectOptions<T> : OutputObjectOptions { }
}
