// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Draco.Core.ObjectStorage.Interfaces
{
    /// <summary>
    /// Defines common options used to create execution object accessor URLs.
    /// For more information on execution objects, see /doc/architecture/execution-objects.md.
    /// </summary>
    public interface IObjectUrlOptions
    {
        TimeSpan DefaultUrlExpirationPeriod { get; }
    }
}
