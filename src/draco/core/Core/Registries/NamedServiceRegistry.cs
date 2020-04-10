// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Core.Registries
{
    /// <summary>
    /// A generic, dictionary-based mechanism for registering services with named service factories.
    /// </summary>
    /// <typeparam name="TService">The type of service that the target factory creates</typeparam>
    public class NamedServiceRegistry<TService> : Dictionary<string, Func<IServiceProvider, TService>>, INamedServiceRegistry<TService>
    {
    }
}
