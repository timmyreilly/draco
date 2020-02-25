// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Draco.Core.ObjectStorage.Interfaces
{
    public interface IObjectUrlOptions
    {
        TimeSpan DefaultUrlExpirationPeriod { get; }
    }
}
