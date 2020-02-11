using Draco.Core.Hosting.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Draco.Core.Hosting.Extensions
{
    public static class ServiceCollectionExtensions
    {
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

            new TModule().ConfigureServices(services, configuration);

            return services;
        }
    }
}
