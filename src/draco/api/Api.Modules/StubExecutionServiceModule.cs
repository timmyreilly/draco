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
    /// <summary>
    /// This service module wires up all the dependencies needed to stand up "stub" extension service infrastructure.
    /// It has no external service dependencies and should be used ONLY in extension service infrastructure integration/end-to-end testing scenarios.
    /// For more information on extension services, see /doc/architecture/extension-services.md.
    /// </summary>
    public class StubExecutionServiceModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IExecutionServiceProvider, CompositeExecutionServiceProvider>();
            services.AddTransient<StubExecutionServiceProvider>();
        }
    }
}
