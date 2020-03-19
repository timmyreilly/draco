// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Api.InternalModels.Extensions;
using Draco.Core.Interfaces;
using Draco.Core.Models;
using Draco.Core.Services.Interfaces;
using Draco.Core.Services.Providers;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Draco.Api.Proxies
{
    /// <summary>
    /// This provider acts a proxy to an extension service provider API (/src/draco/api/ExtensionService.Api) endpoint.
    /// For more information on extension services, see /doc/architecture/extension-services.md.
    /// </summary>
    public class ProxyExecutionServiceProvider : BaseExecutionServiceProvider, IExecutionServiceProvider
    {
        private readonly IJsonHttpClient jsonHttpClient;
        private readonly ProxyConfiguration proxyConfig;

        public ProxyExecutionServiceProvider(IJsonHttpClient jsonHttpClient, ProxyConfiguration proxyConfig)
        {
            this.jsonHttpClient = jsonHttpClient;
            this.proxyConfig = proxyConfig;
        }

        public async override Task<JObject> GetServiceConfigurationAsync(ExecutionRequest execRequest)
        {
            if (execRequest == null)
            {
                throw new ArgumentNullException(nameof(execRequest));
            }

            var apiModel = execRequest.ToApiModel();
            var apiUrl = $"{proxyConfig.BaseUrl.TrimEnd('/')}/config-request";
            var apiResponse = await jsonHttpClient.PostAsync<JObject>(apiUrl, apiModel);

            switch (apiResponse.StatusCode)
            {
                case HttpStatusCode.NoContent:
                    return null;
                case HttpStatusCode.OK:
                    return apiResponse.Content;
                default:
                    throw new HttpRequestException($"[Request {execRequest.ExecutionId}]: " +
                                                   $"Execution service API [{apiUrl}] responded with an unexpected status code: [{apiResponse.StatusCode}].");
            }
        }
    }
}
