// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.Interfaces;
using Core.Registries;
using Draco.Core.Factories;
using Draco.Core.Hosting.Interfaces;
using Draco.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Modules
{
    /// <summary>
    /// This is a convenience base class for creating service modules that configure named service factories.
    /// Inherited classes must override the [AddNamedServices] method to map service names to factory functions.
    /// </summary>
    /// <typeparam name="TService">The type of service that the inherited service module creates</typeparam>
    public abstract class BaseNamedServiceModule<TService> : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Create a fresh service registry...
            var serviceRegistry = new NamedServiceRegistry<TService>();

            // The inherited class maps service names to factory functions...
            AddNamedServices(configuration, serviceRegistry);

            // Register the named service factory for injection by the host application...
            services.AddSingleton<INamedServiceFactory<TService>>(new NamedServiceFactory<TService>(serviceRegistry));
        }

        public abstract void AddNamedServices(IConfiguration configuration, INamedServiceRegistry<TService> serviceRegistry);
    }
}
