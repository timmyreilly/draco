// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Core.Interfaces;
using Core.Modules;
using Draco.Azure.ObjectStorage.Constants;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.Execution.Api.Modules.Factories
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
            serviceRegistry[AzureObjectStorageProviders.BlobStorage.V1] = sp => sp.GetService<InputObjectUrlAccessorProvider>(); // az-blob/v1
        }
    }
}
