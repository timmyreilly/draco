// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Hosting.Interfaces;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.Api.Modules
{
    /// <summary>
    /// This service module wires up all the core object storage dependencies needed as part of the standard execution pipeline. 
    /// These common dependencies will be neeeded regardless of which storage platform(s) you're using.
    /// 
    /// This service module is platform-agnostic and will need to be configured alongside platform-specific (Azure, AWS, etc.)
    /// service modules that wire up dependencies needed for platform-level storage support (Azure blobs, AWS S3, NFS, SMB, etc.)
    /// 
    /// For more information on object storage, see /doc/architecture/execution-objects.md.
    /// </summary>
    public class CoreObjectStorageModule : IServiceModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<InputObjectUrlAccessorProvider>();
            services.AddTransient<OutputObjectUrlAccessorProvider>();

            services.AddTransient<IInputObjectAccessorProvider, CompositeInputObjectAccessorProvider>();
            services.AddTransient<IOutputObjectAccessorProvider, CompositeOutputObjectAccessorProvider>();
        }
    }
}
