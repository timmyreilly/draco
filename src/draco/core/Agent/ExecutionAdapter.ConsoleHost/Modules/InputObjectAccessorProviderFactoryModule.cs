// Copyright (c) Microsoft Corporation.
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
    public class InputObjectAccessorProviderFactoryModule : BaseNamedServiceModule<IInputObjectAccessorProvider>
    {
        public override void AddNamedServices(IConfiguration configuration, INamedServiceRegistry<IInputObjectAccessorProvider> serviceRegistry)
        {
            serviceRegistry[AzureObjectStorageProviders.BlobStorage.V1] = sp => sp.GetService<InputObjectUrlAccessorProvider>();
        }
    }
}
