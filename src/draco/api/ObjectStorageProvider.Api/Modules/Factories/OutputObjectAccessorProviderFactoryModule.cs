﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.Interfaces;
using Core.Modules;
using Draco.Core.ObjectStorage.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Draco.ObjectStorageProvider.Api.Modules.Factories
{
    /// <summary>
    /// This service module maps object storage provider names to output object accessor providers.
    /// As you onboard new object storage providers, you will need to register the appropriate output object accessor providers here.
    /// For more information on object accessors, see /doc/architecture/execution-objects.md#object-accessors.
    /// </summary>
    public class OutputObjectAccessorProviderFactoryModule : BaseNamedServiceModule<IOutputObjectAccessorProvider>
    {
        public override void AddNamedServices(IConfiguration configuration, INamedServiceRegistry<IOutputObjectAccessorProvider> serviceRegistry)
        {
            // TODO: Register output object accessor providers here...
            // serviceRegistry["stub/v1"] = sp => sp.GetService<StubOutputObjectAccessorProvider>();
        }
    }
}
