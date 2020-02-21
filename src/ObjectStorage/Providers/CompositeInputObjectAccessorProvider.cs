// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Interfaces;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Draco.Core.ObjectStorage.Providers
{
    public class CompositeInputObjectAccessorProvider : IInputObjectAccessorProvider
    {
        private readonly IServiceProvider serviceProvider;
        private readonly INamedServiceFactory<IInputObjectAccessorProvider> accessorProviderFactory;

        public CompositeInputObjectAccessorProvider(IServiceProvider serviceProvider,
                                                    INamedServiceFactory<IInputObjectAccessorProvider> accessorProviderFactory)
        {
            this.serviceProvider = serviceProvider;
            this.accessorProviderFactory = accessorProviderFactory;
        }

        public async Task<JObject> GetReadableAccessorAsync(InputObjectAccessorRequest accessorRequest)
        {
            var accessorProvider = GetAccessorProvider(accessorRequest);
            var accessor = await accessorProvider.GetReadableAccessorAsync(accessorRequest);

            return accessor;
        }

        public async Task<JObject> GetWritableAccessorAsync(InputObjectAccessorRequest accessorRequest)
        {
            var accessorProvider = GetAccessorProvider(accessorRequest);
            var accessor = await accessorProvider.GetWritableAccessorAsync(accessorRequest);

            return accessor;
        }

        private IInputObjectAccessorProvider GetAccessorProvider(InputObjectAccessorRequest accessorRequest)
        {
            if (accessorRequest == null)
            {
                throw new ArgumentNullException(nameof(accessorRequest));
            }

            if (accessorProviderFactory.ContainsKey(accessorRequest.ObjectProviderName) == false)
            {
                throw new NotSupportedException($"Object provider [{accessorRequest.ObjectProviderName}] not supported.");
            }

            return accessorProviderFactory[accessorRequest.ObjectProviderName](serviceProvider);
        }
    }
}
