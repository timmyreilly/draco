// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Api.InternalModels;
using Draco.Api.InternalModels.Extensions;
using Draco.Core.Execution.Interfaces;
using Draco.Core.Interfaces;
using Draco.Core.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Draco.Api.Proxies
{
    public class ProxyExecutionAdapter : IExecutionAdapter
    {
        private readonly IJsonHttpClient jsonHttpClient;
        private readonly ProxyConfiguration proxyConfig;

        public ProxyExecutionAdapter(IJsonHttpClient jsonHttpClient, ProxyConfiguration proxyConfig)
        {
            this.jsonHttpClient = jsonHttpClient;
            this.proxyConfig = proxyConfig;
        }

        public async Task<Core.Models.ExecutionContext> ExecuteAsync(ExecutionRequest request, CancellationToken cancelToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (cancelToken == null)
            {
                throw new ArgumentNullException(nameof(cancelToken));
            }

            var apiModel = request.ToApiModel();
            var apiUrl = $"{proxyConfig.BaseUrl.TrimEnd('/')}/";
            var apiResponse = await jsonHttpClient.PostAsync<ExecutionContextApiModel>(apiUrl, apiModel);

            switch (apiResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return apiResponse.Content.ToCoreModel();
                default:
                    throw new HttpRequestException($"[Request {request.ExecutionId}]: " +
                                                   $"Execution adapter API [{apiUrl}] responded with an unexpected status code: [{apiResponse.StatusCode}].");
            }
        }
    }
}
