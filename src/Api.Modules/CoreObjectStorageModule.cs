// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Hosting.Interfaces;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.Api.Modules
{
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
