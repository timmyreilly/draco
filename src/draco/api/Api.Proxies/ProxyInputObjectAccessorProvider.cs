﻿// Copyright (c) Microsoft Corporation.
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
    /// <summary>
    /// This provider acts as a proxy to (input) object provider API (/src/draco/api/ObjectStorageProvider.Api) endpoints.
    /// For more information on object providers, see /doc/architecture/execution-objects.md#object-providers.
    /// </summary>
    public class ProxyInputObjectAccessorProvider : IInputObjectAccessorProvider
    {
        private readonly IJsonHttpClient jsonHttpClient;
        private readonly ProxyConfiguration proxyConfig;

        public ProxyInputObjectAccessorProvider(IJsonHttpClient jsonHttpClient, ProxyConfiguration proxyConfig)
        {
            this.jsonHttpClient = jsonHttpClient;
            this.proxyConfig = proxyConfig;
        }

        public async Task<JObject> GetReadableAccessorAsync(InputObjectAccessorRequest accessorRequest)
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
                                                   $"[Input Object {accessorRequest.ObjectMetadata.Name}]: " +
                                                   $"Object provider API [{apiUrl}] responded with an unexpected status code: [{apiResponse.StatusCode}].");
            }
        }

        public async Task<JObject> GetWritableAccessorAsync(InputObjectAccessorRequest accessorRequest)
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
                                                   $"[Input Object {accessorRequest.ObjectMetadata.Name}]: " +
                                                   $"Object provider API [{apiUrl}] responded with an unexpected status code: [{apiResponse.StatusCode}].");
            }
        }
    }
}
