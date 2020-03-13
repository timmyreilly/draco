// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.Services.Providers;
using Draco.Core.Hosting.Interfaces;
using Draco.Core.Services.Interfaces;
using Draco.Core.Services.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Modules
{
    public class StubExecutionServiceModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IExecutionServiceProvider, CompositeExecutionServiceProvider>();

            services.AddTransient<StubExecutionServiceProvider>();
        }
    }
}
