// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Hosting.Interfaces;
using Draco.Core.Interfaces;
using Draco.Core.ObjectStorage.Models;
using Draco.Core.ObjectStorage.Services;
using Draco.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ObjectStorageProvider.Api.Modules
{
    /// <summary>
    /// This service module wires up all the dependencies needed to sign object accessors.
    /// </summary>
    public class SignerModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ISigner<ObjectUrl>, ObjectUrlSigner>();
            services.AddTransient<ISigner, DefaultSigner>();
        }
    }
}
