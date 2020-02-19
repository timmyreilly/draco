// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Models;
using Draco.Core.Execution.Services;
using Draco.Core.Hosting.Interfaces;
using Draco.Core.Interfaces;
using Draco.Core.ObjectStorage.Models;
using Draco.Core.ObjectStorage.Services;
using Draco.Core.Services;
using Draco.Execution.Api.Models;
using Draco.Execution.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.Execution.Api.Modules
{
    public class SignerModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISigner<HttpExecutionRequest>, HttpExecutionRequestSigner>();
            services.AddTransient<ISigner<DirectExecutionRequestApiModel>, DirectExecutionRequestApiModelSigner>();
            services.AddTransient<ISigner<ObjectUrl>, ObjectUrlSigner>();
            services.AddTransient<ISigner, DefaultSigner>();
        }
    }
}
