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
    /// This service module wires up all the dependencies needed to stand up "stub" object storage infrastructure.
    /// It has no external service dependencies and should be used ONLY in object storage infrastructure integration/end-to-end testing scenarios.
    /// For more information on object storage, see /doc/architecture/execution-objects.md.
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
