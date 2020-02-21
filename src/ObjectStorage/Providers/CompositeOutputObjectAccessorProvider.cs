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
    public class CompositeOutputObjectAccessorProvider : IOutputObjectAccessorProvider
    {
        private readonly IServiceProvider serviceProvider;
        private readonly INamedServiceFactory<IOutputObjectAccessorProvider> accessorProviderFactory;

        public CompositeOutputObjectAccessorProvider(IServiceProvider serviceProvider,
                                                     INamedServiceFactory<IOutputObjectAccessorProvider> accessorProviderFactory)
        {
            this.serviceProvider = serviceProvider;
            this.accessorProviderFactory = accessorProviderFactory;
        }

        public async Task<JObject> GetReadableAccessorAsync(OutputObjectAccessorRequest accessorRequest)
        {
            var accessorProvider = GetAccessorProvider(accessorRequest);
            var accessor = await accessorProvider.GetReadableAccessorAsync(accessorRequest);

            return accessor;
        }

        public async Task<JObject> GetWritableAccessorAsync(OutputObjectAccessorRequest accessorRequest)
        {
            var accessorProvider = GetAccessorProvider(accessorRequest);
            var accessor = await accessorProvider.GetWritableAccessorAsync(accessorRequest);

            return accessor;
        }

        private IOutputObjectAccessorProvider GetAccessorProvider(OutputObjectAccessorRequest accessorRequest)
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
