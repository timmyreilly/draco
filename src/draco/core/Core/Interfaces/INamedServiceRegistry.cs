// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Core.Interfaces
{
    /// <summary>
    /// Defines a generic, dictionary-based mechanism for registering services with named service factories.
    /// </summary>
    /// <typeparam name="TService">The type of service that the target factory creates</typeparam>
    public interface INamedServiceRegistry<TService> : IDictionary<string, Func<IServiceProvider, TService>> { }
}
