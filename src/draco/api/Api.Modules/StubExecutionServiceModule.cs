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
    /// This service module wires up all the stub extension service dependencies needed to host the default execution pipeline in a dev/test environment.
    /// Note that this module should be used only in dev/test scenarios and is not meant for production.
    /// For more information on extension services, see /doc/architecture/extension-services.md.
    /// For more information on the execution pipeline, see /doc/architecture/execution-pipeline.md.
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
