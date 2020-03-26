// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Hosting.Interfaces;
using Draco.Core.Models.Interfaces;
using Draco.Execution.Api.Interfaces;
using Draco.Execution.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.Execution.Api.Modules
{
    /// <summary>
    /// This service module wires up all the core execution API-specific dependencies.
    /// </summary>
    public class CoreExecutionApiModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IExecutionRequestContextBuilder, ExecutionRequestContextBuilder>();
            services.AddTransient<IExtensionObjectApiModelService, ExtensionObjectApiModelService>();
            services.AddTransient<IExtensionRsaKeyProvider, TestExtensionRsaKeyProvider>();
        }
    }
}
