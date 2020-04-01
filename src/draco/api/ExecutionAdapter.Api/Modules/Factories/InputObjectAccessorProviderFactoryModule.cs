// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.Interfaces;
using Core.Modules;
using Core.ObjectStorage.Providers;
using Draco.Core.ObjectStorage.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.ExecutionAdapter.Api.Modules.Factories
{
    /// <summary>
    /// This service module maps object storage provider names to input object accessor providers.
    /// As you onboard new object storage providers, you will need to register the appropriate input object accessor providers here.
    /// For more information on object storage, see /doc/architecture/execution-objects.md#object-accessors.
    /// </summary>
    public class InputObjectAccessorProviderFactoryModule : BaseNamedServiceModule<IInputObjectAccessorProvider>
    {
        public override void AddNamedServices(IConfiguration configuration, INamedServiceRegistry<IInputObjectAccessorProvider> serviceRegistry)
        {
            serviceRegistry["stub/v1"] = sp => sp.GetService<StubInputObjectAccessorProvider>();
        }
    }
}
