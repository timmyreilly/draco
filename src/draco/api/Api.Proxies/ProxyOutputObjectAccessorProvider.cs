// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Api.InternalModels.Extensions;
using Draco.Core.Interfaces;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Draco.Api.Proxies
{
    public class ProxyOutputObjectAccessorProvider : IOutputObjectAccessorProvider
    {
        private readonly IJsonHttpClient jsonHttpClient;
        private readonly ProxyConfiguration proxyConfig;

        public ProxyOutputObjectAccessorProvider(IJsonHttpClient jsonHttpClient, ProxyConfiguration proxyConfig)
        {
            this.jsonHttpClient = jsonHttpClient;
            this.proxyConfig = proxyConfig;
        }

        public async Task<JObject> GetReadableAccessorAsync(OutputObjectAccessorRequest accessorRequest)
        {
            if (accessorRequest == null)
            {
                throw new ArgumentNullException(nameof(accessorRequest));
            }

            var apiModel = accessorRequest.ToApiModel();
            var apiUrl = $"{proxyConfig.BaseUrl.TrimEnd('/')}/readable";
            var apiResponse = await jsonHttpClient.PostAsync<JObject>(apiUrl, apiModel);

            switch (apiResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return apiResponse.Content;
                default:
                    throw new HttpRequestException($"[Request {accessorRequest.ExecutionMetadata.ExecutionId}]: " +
                                                   $"[Output Object {accessorRequest.ObjectMetadata.Name}]: " +
                                                   $"Object provider API [{apiUrl}] responded with an unexpected status code: [{apiResponse.StatusCode}].");
            }
        }

        public async Task<JObject> GetWritableAccessorAsync(OutputObjectAccessorRequest accessorRequest)
        {
            if (accessorRequest == null)
            {
                throw new ArgumentNullException(nameof(accessorRequest));
            }

            var apiModel = accessorRequest.ToApiModel();
            var apiUrl = $"{proxyConfig.BaseUrl.TrimEnd('/')}/writable";
            var apiResponse = await jsonHttpClient.PostAsync<JObject>(apiUrl, apiModel);

            switch (apiResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return apiResponse.Content;
                default:
                    throw new HttpRequestException($"[Request {accessorRequest.ExecutionMetadata.ExecutionId}]: " +
                                                   $"[Output Object {accessorRequest.ObjectMetadata.Name}]: " +
                                                   $"Object provider API [{apiUrl}] responded with an unexpected status code: [{apiResponse.StatusCode}].");
            }
        }
    }
}
