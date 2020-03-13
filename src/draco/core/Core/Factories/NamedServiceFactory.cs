// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Draco.Core.Factories
{
    public class NamedServiceFactory<TService> : INamedServiceFactory<TService>
    {
        private readonly Dictionary<string, Func<IServiceProvider, TService>> factoryFuncDictionary = new Dictionary<string, Func<IServiceProvider, TService>>();
        private readonly ReaderWriterLockSlim factoryLock = new ReaderWriterLockSlim();

        public NamedServiceFactory() { }

        public NamedServiceFactory(IDictionary<string, Func<IServiceProvider, TService>> factoryFuncDictionary)
        {
            this.factoryFuncDictionary = new Dictionary<string, Func<IServiceProvider, TService>>(factoryFuncDictionary);
        }

        public IEnumerable<string> Keys
        {
            get
            {
                try
                {
                    this.factoryLock.EnterReadLock();

                    return this.factoryFuncDictionary.Keys;
                }
                finally
                {
                    this.factoryLock.ExitReadLock();
                }
            }
        }

        public bool CanCreateService(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            try
            {
                this.factoryLock.EnterReadLock();

                return this.factoryFuncDictionary.ContainsKey(serviceName);
            }
            finally
            {
                this.factoryLock.ExitReadLock();
            }
        }

        public TService CreateService(string serviceName, IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            try
            {
                this.factoryLock.EnterReadLock();

                if (this.factoryFuncDictionary.ContainsKey(serviceName))
                {
                    return this.factoryFuncDictionary[serviceName](serviceProvider);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(serviceName), $"Service [{serviceName}] not registered with this factory.");
                }
            }
            finally
            {
                this.factoryLock.ExitReadLock();
            }
        }

        public void RegisterService(string serviceName, Func<IServiceProvider, TService> factoryFunc)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            if (factoryFunc == null)
            {
                throw new ArgumentNullException(nameof(factoryFunc));
            }

            try
            {
                this.factoryLock.EnterWriteLock();
                this.factoryFuncDictionary[serviceName] = factoryFunc;
            }
            finally
            {
                this.factoryLock.ExitWriteLock();
            }
        }

        public void RegisterServices(IDictionary<string, Func<IServiceProvider, TService>> factoryFuncDictionary)
        {
            if (factoryFuncDictionary == null)
            {
                throw new ArgumentNullException(nameof(factoryFuncDictionary));
            }

            try
            {
                this.factoryLock.EnterWriteLock();

                foreach (var serviceName in factoryFuncDictionary.Keys)
                {
                    this.factoryFuncDictionary[serviceName] = factoryFuncDictionary[serviceName];
                }
            }
            finally
            {
                this.factoryLock.ExitWriteLock();
            }
        }
    }
}
