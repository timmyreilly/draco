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
    public class OutputObjectUrlAccessorProvider : IOutputObjectAccessorProvider
    {
        private readonly IOutputObjectUrlProvider urlProvider;
        private readonly ISigner<ObjectUrl> urlSigner;

        public OutputObjectUrlAccessorProvider(IOutputObjectUrlProvider urlProvider,
                                               ISigner<ObjectUrl> urlSigner)
        {
            this.urlProvider = urlProvider;
            this.urlSigner = urlSigner;
        }

        public async Task<JObject> GetReadableAccessorAsync(OutputObjectAccessorRequest accessorRequest)
        {
            var urlRequest = CreateUrlRequest(accessorRequest);
            var url = await urlProvider.GetReadableUrlAsync(urlRequest);

            if (!string.IsNullOrEmpty(accessorRequest.SignatureRsaKeyXml))
            {
                url.Signature = await urlSigner.GenerateSignatureAsync(accessorRequest.SignatureRsaKeyXml, url);
            }

            return JObject.FromObject(url);
        }

        public async Task<JObject> GetWritableAccessorAsync(OutputObjectAccessorRequest accessorRequest)
        {
            var urlRequest = CreateUrlRequest(accessorRequest);
            var url = await urlProvider.GetWritableUrlAsync(urlRequest);

            if (!string.IsNullOrEmpty(accessorRequest.SignatureRsaKeyXml))
            {
                url.Signature = await urlSigner.GenerateSignatureAsync(accessorRequest.SignatureRsaKeyXml, url);
            }

            return JObject.FromObject(url);
        }

        private ObjectUrlRequest CreateUrlRequest(OutputObjectAccessorRequest accessorRequest)
        {
            if (accessorRequest == null)
            {
                throw new ArgumentNullException(nameof(accessorRequest));
            }

            return new ObjectUrlRequest
            {
                ExecutionMetadata = accessorRequest.ExecutionMetadata,
                ObjectName = accessorRequest.ObjectMetadata.Name,
                UrlExpirationPeriod = accessorRequest.ExpirationPeriod
            };
        }
    }
}
