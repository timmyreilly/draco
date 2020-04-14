// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Hosting.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Draco.Core.Hosting.Extensions
{
    /// <summary>
    /// Extension methods needed to work with service collections and modules.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures all the services defined in [TModule].
        /// </summary>
        /// <param name="services">The hosting application's service collection</param>
        /// <param name="configuration">The hosting application's configuration</param>
        /// <typeparam name="TModule">The type of module to configure</typeparam>
        /// <returns></returns>
        public static IServiceCollection ConfigureServices<TModule>(this IServiceCollection services, IConfiguration configuration)
            where TModule : IServiceModule, new()
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            // Create a service module [TModule] and register its services with the hosting application's service collection.

            new TModule().ConfigureServices(services, configuration);

            return services;
        }
    }
}
