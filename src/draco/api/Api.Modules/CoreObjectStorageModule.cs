// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Hosting.Interfaces;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.Api.Modules
{
    /// <summary>
    /// This service module wires up all the core object storage dependencies needed as part of the default execution pipeline.
    /// For more information, see /doc/architecture/execution-objects.md.
    /// </summary>
    public class CoreObjectStorageModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<InputObjectUrlAccessorProvider>();
            services.AddTransient<OutputObjectUrlAccessorProvider>();

            services.AddTransient<IInputObjectAccessorProvider, CompositeInputObjectAccessorProvider>();
            services.AddTransient<IOutputObjectAccessorProvider, CompositeOutputObjectAccessorProvider>();
        }
    }
}
