// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface INamedServiceRegistry<TService> : IDictionary<string, Func<IServiceProvider, TService>> { }
}
