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
    public abstract class BaseNamedServiceModule<TService> : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var serviceRegistry = new NamedServiceRegistry<TService>();

            AddNamedServices(configuration, serviceRegistry);

            services.AddSingleton<INamedServiceFactory<TService>>(new NamedServiceFactory<TService>(serviceRegistry));
        }

        public abstract void AddNamedServices(IConfiguration configuration, INamedServiceRegistry<TService> serviceRegistry);
    }
}
