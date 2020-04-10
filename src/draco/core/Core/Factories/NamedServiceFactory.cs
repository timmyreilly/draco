// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Draco.Core.Factories
{
    /// <summary>
    /// A generic, concurrent, platform-agnostic mechanism for creating and managing named services. 
    /// This factory should be injected into host applications as a singleton.
    /// </summary>
    /// <typeparam name="TService">The type of service this factory creates</typeparam>
    public class NamedServiceFactory<TService> : INamedServiceFactory<TService>
    {
        // This is the internal dictionary that maps service names to functions that can create those services.
        private readonly Dictionary<string, Func<IServiceProvider, TService>> factoryFuncDictionary = new Dictionary<string, Func<IServiceProvider, TService>>();

        // Access to the internal dictionary [factoryFuncDictionary] is protected by this read/write lock.
        private readonly ReaderWriterLockSlim factoryLock = new ReaderWriterLockSlim();

        public NamedServiceFactory() { }

        public NamedServiceFactory(IDictionary<string, Func<IServiceProvider, TService>> factoryFuncDictionary)
        {
            // Initialize this factory with an existing function dictionary...

            this.factoryFuncDictionary = new Dictionary<string, Func<IServiceProvider, TService>>(factoryFuncDictionary);
        }

        /// <summary>
        /// Gets all the service names registered with this factory
        /// </summary>
        /// <value></value>
        public IEnumerable<string> Keys
        {
            get
            {
                // Using [factoryLock], multiple application threads can access this property at the same time 
                // while service dictionary [factoryFuncDictionary] updates (writes) are temporarily suspended.

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

        /// <summary>
        /// Checks to see if a specific service is registered with this factory
        /// </summary>
        /// <param name="serviceName">The service name</param>
        /// <returns></returns>
        public bool CanCreateService(string serviceName)
        {
            // Using the [factoryLock], multiple application threads can call this method at the same time
            // while service dictionary [factoryFuncDictionary] updates (writes) are temporarily suspended.

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

        /// <summary>
        /// Creates a named service
        /// </summary>
        /// <param name="serviceName">The service name</param>
        /// <param name="serviceProvider">The hosting application's service provider</param>
        /// <returns></returns>
        public TService CreateService(string serviceName, IServiceProvider serviceProvider)
        {
            // Using the [factoryLock], multiple application threads can call this method at the same time
            // while service dictionary [factoryFuncDictionary] updates (writes) are temporarily suspended.

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

                // Check to make sure that the service is actually registered...
                if (this.factoryFuncDictionary.ContainsKey(serviceName))
                {
                    // If it is, call the right function...
                    return this.factoryFuncDictionary[serviceName](serviceProvider);
                }
                else
                {
                    // If not, throw an exception...
                    throw new ArgumentOutOfRangeException(nameof(serviceName), $"Service [{serviceName}] not registered with this factory.");
                }
            }
            finally
            {
                this.factoryLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Registers a new service with this factory.
        /// </summary>
        /// <param name="serviceName">The name of the service</param>
        /// <param name="factoryFunc">The function that creates this service</param>
        public void RegisterService(string serviceName, Func<IServiceProvider, TService> factoryFunc)
        {
            // Only one application thread at a time can call this function and update the service dictionary [factoryFuncDictionary].
            // Reads from the dictionary are temporarily suspended. 

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

        /// <summary>
        /// Registers multiple services with this factory
        /// </summary>
        /// <param name="factoryFuncDictionary">The services to register</param>
        public void RegisterServices(IDictionary<string, Func<IServiceProvider, TService>> factoryFuncDictionary)
        {
            // Only one application thread at a time can call this function and update the service dictionary [factoryFuncDictionary].
            // Reads from the dictionary are temporarily suspended. 

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
