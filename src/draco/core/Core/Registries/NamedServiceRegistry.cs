// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Core.Registries
{
    public class NamedServiceRegistry<TService> : Dictionary<string, Func<IServiceProvider, TService>>, INamedServiceRegistry<TService>
    {
    }
}
