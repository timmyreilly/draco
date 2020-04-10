// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Hosting.Interfaces;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ExecutionAdapter.ConsoleHost.Modules
{
    /// <summary>
    /// Wires up all the platform-agnostic dependencies needed to support URL-based execution object accessors.
    /// For more information on object accessors, see /doc/architecture/execution-objects.md#object-accessors.
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
