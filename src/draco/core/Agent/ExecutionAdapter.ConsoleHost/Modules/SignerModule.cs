// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Models;
using Draco.Core.Execution.Services;
using Draco.Core.Hosting.Interfaces;
using Draco.Core.Interfaces;
using Draco.Core.ObjectStorage.Models;
using Draco.Core.ObjectStorage.Services;
using Draco.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ExecutionAdapter.ConsoleHost.Modules
{
    /// <summary>
    /// This service module wires up all the dependencies needed to sign execution requests and object accessors.
    /// </summary>
    public class SignerModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISigner<HttpExecutionRequest>, HttpExecutionRequestSigner>();
            services.AddTransient<ISigner<ObjectUrl>, ObjectUrlSigner>();
            services.AddTransient<ISigner, DefaultSigner>();
        }
    }
}
