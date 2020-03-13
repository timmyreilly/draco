// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Draco.Core.Interfaces
{
    public interface INamedServiceFactory<TService>
    {
        IEnumerable<string> Keys { get; }
       
        bool CanCreateService(string serviceName);
        TService CreateService(string serviceName, IServiceProvider serviceProvider);
        void RegisterService(string serviceName, Func<IServiceProvider, TService> factoryFunc);
        void RegisterServices(IDictionary<string, Func<IServiceProvider, TService>> factoryFuncDictionary);
    }
}
