// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.Interfaces;
using Core.Modules;
using Core.ObjectStorage.Providers;
using Draco.Core.ObjectStorage.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ObjectStorageProvider.Api.Modules.Factories
{
    public class OutputObjectAccessorProviderFactoryModule : BaseNamedServiceModule<IOutputObjectAccessorProvider>
    {
        public override void AddNamedServices(IConfiguration configuration, INamedServiceRegistry<IOutputObjectAccessorProvider> serviceRegistry)
        {
            serviceRegistry["stub/v1"] = sp => sp.GetService<StubOutputObjectAccessorProvider>();
        }
    }
}
