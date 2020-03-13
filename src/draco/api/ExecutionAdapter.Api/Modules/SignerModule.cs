// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Hosting.Interfaces;
using Draco.Core.Interfaces;
using Draco.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ExecutionAdapter.Api.Modules
{
    public class SignerModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISigner, DefaultSigner>();
        }
    }
}
