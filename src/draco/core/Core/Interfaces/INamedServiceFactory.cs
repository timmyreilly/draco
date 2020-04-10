// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Draco.Core.Interfaces
{
    /// <summary>
    /// Defines a generic mechanism for creating and managing named services.
    /// </summary>
    /// <typeparam name="TService">The type of service that the factory creates</typeparam>
    public interface INamedServiceFactory<TService>
    {
        IEnumerable<string> Keys { get; }
       
        bool CanCreateService(string serviceName);
        TService CreateService(string serviceName, IServiceProvider serviceProvider);
        void RegisterService(string serviceName, Func<IServiceProvider, TService> factoryFunc);
        void RegisterServices(IDictionary<string, Func<IServiceProvider, TService>> factoryFuncDictionary);
    }
}
