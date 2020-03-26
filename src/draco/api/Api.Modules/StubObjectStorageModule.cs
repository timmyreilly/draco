// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.ObjectStorage.Providers;
using Draco.Core.Hosting.Interfaces;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Modules
{
    /// <summary>
    /// This service module wires up all the stub execution object dependencies needed to host the default execution pipeline in a dev/test environment.
    /// Note that this module should be used only in dev/test scenarios and is not meant for production.
    /// For more information on execution objects, see /doc/architecture/execution-objects.md.
    /// For more information on the execution pipeline, see /doc/architecture/execution-pipeline.md.
    /// </summary>
    public class StubObjectStorageModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IInputObjectAccessorProvider, CompositeInputObjectAccessorProvider>();
            services.AddTransient<IOutputObjectAccessorProvider, CompositeOutputObjectAccessorProvider>();

            services.AddTransient<StubInputObjectAccessorProvider>();
            services.AddTransient<StubOutputObjectAccessorProvider>();
        }
    }
}
