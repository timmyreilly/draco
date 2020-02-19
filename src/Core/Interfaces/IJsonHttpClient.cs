// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Draco.Core.Interfaces
{
    public interface IJsonHttpClient
    {
        Task<HttpResponse<TResponse>> GetAsync<TResponse>(string url);
        Task<HttpResponse> PostAsync(string url, object requestData);
        Task<HttpResponse<TResponse>> PostAsync<TResponse>(string url, object requestData);
        Task<HttpResponse> PutAsync(string url, object requestData);
        Task<HttpResponse<TResponse>> PutAsync<TResponse>(string url, object requestData);
    }
}
