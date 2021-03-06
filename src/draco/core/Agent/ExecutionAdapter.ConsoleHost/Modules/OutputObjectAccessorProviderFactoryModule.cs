﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.Interfaces;
using Core.Modules;
using Draco.Azure.ObjectStorage.Constants;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ExecutionAdapter.ConsoleHost.Modules
{
    /// <summary>
    /// This module maps object provider names to output object accessor providers that can handle them.
    /// For more information on object providers, see /doc/architecture/execution-objects.md.
    /// </summary>
    public class OutputObjectAccessorProviderFactoryModule : BaseNamedServiceModule<IOutputObjectAccessorProvider>
    {
        public override void AddNamedServices(IConfiguration configuration, INamedServiceRegistry<IOutputObjectAccessorProvider> serviceRegistry)
        {
            serviceRegistry[AzureObjectStorageProviders.BlobStorage.V1] = sp => sp.GetService<OutputObjectUrlAccessorProvider>();
        }
    }
}
