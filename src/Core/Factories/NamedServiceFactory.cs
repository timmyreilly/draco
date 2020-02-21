// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Draco.Core.Factories
{
    public class NamedServiceFactory<TService> : Dictionary<string, Func<IServiceProvider, TService>>,
                                                 INamedServiceFactory<TService>
    { }
}