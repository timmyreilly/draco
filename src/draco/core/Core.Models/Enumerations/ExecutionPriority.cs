// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Draco.Core.Models.Enumerations
{
    [Flags]
    public enum ExecutionPriority
    {
        Undefined = 0,
        Low = 1,
        Normal = 2,
        High = 4
    }
}
