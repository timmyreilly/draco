// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Hosting.Interfaces;
using Draco.IntegrationTests.HowdyService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.Api.Modules.ExtensionServices
{
    /// <summary>
    /// Service module needed to wire up the sample (not meant for production!) "howdy/v1" extension service -- /src/draco/tests/IntegrationTests.HowdyService
    /// Should be used only for integration/end-to-end testing
    /// </summary>
    public class HowdyExecutionServiceModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<HowdyServiceProvider>();
        }
    }
}
