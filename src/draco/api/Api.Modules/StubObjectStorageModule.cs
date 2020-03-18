﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.ObjectStorage.Providers;
using Draco.Core.Hosting.Interfaces;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Modules
{
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