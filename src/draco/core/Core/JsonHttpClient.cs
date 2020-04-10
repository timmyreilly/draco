// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Extensions;
using Draco.Core.Interfaces;
using Draco.Core.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Draco.Core
{
    /// <summary>
    /// A simple, generic client for working with JSON-based REST APIs.
    /// </summary>
    public class JsonHttpClient : IJsonHttpClient
    {
        private const string JsonMediaType = "application/json";

        private readonly HttpClient httpClient;
        private readonly RetryPolicy httpRetryPolicy;

        public JsonHttpClient(IOptionsSnapshot<HttpClientOptions<JsonHttpClient>> optionsSnapshot)
            : this(optionsSnapshot.Value) { }

        public JsonHttpClient(IHttpClientOptions clientOptions)
        {
            // Create the inner HTTP client...
            this.httpClient = new HttpClient();

            // Configure the retry policy...
            this.httpRetryPolicy = Policy.Handle<HttpRequestException>()
                .ConfigureExponentialBackOffRetryPolicy(clientOptions.MaximumRetryAttempts);

            // Always include the "application/json" accept header...
            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMediaType));
        }

        public async Task<HttpResponse<TResponse>> GetAsync<TResponse>(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (Uri.TryCreate(url, UriKind.Absolute, out _) == false)
            {
                throw new ArgumentException($"[{url}] is not a valid absolute URL.", nameof(url));
            }

            var httpResponse = await httpRetryPolicy.Execute(async () => EnsureSuccessStatusCode(await httpClient.GetAsync(url)));

            return new HttpResponse<TResponse>(httpResponse.StatusCode, await TryToGetResponseData<TResponse>(httpResponse));
        }

        public async Task<HttpResponse> PostAsync(string url, object requestData)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (Uri.TryCreate(url, UriKind.Absolute, out _) == false)
            {
                throw new ArgumentException($"[{url}] is not a valid absolute URL.", nameof(url));
            }

            if (requestData == null)
            {
                throw new ArgumentNullException(nameof(requestData));
            }

            var httpResponse = await httpRetryPolicy.Execute(async () =>
                EnsureSuccessStatusCode(await httpClient.PostAsync(url, ToHttpContent(requestData))));

            return new HttpResponse(httpResponse.StatusCode);
        }

        public async Task<HttpResponse<TResponse>> PostAsync<TResponse>(string url, object requestData)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (Uri.TryCreate(url, UriKind.Absolute, out _) == false)
            {
                throw new ArgumentException($"[{url}] is not a valid absolute URL.", nameof(url));
            }

            if (requestData == null)
            {
                throw new ArgumentNullException(nameof(requestData));
            }

            var httpResponse = await httpRetryPolicy.Execute(async () =>
            {
                try
                {
                    var jsonContent = JsonConvert.SerializeObject(requestData);
                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, JsonMediaType);

                    return EnsureSuccessStatusCode(await httpClient.PostAsync(url, httpContent));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    throw;
                }
            });

            return new HttpResponse<TResponse>(httpResponse.StatusCode, await TryToGetResponseData<TResponse>(httpResponse));
        }

        public async Task<HttpResponse> PutAsync(string url, object requestData)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            if (Uri.TryCreate(url, UriKind.Absolute, out _) == false)
                throw new ArgumentException($"[{url}] is not a valid absolute URL.", nameof(url));

            if (requestData == null)
                throw new ArgumentNullException(nameof(requestData));

            var httpResponse = await httpRetryPolicy.Execute(async () =>
            {
                try
                {
                    return EnsureSuccessStatusCode(await httpClient.PutAsync(url, ToHttpContent(requestData)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    throw;
                }
            });

            return new HttpResponse(httpResponse.StatusCode);
        }

        public async Task<HttpResponse<TResponse>> PutAsync<TResponse>(string url, object requestData)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            if (Uri.TryCreate(url, UriKind.Absolute, out _) == false)
                throw new ArgumentException($"[{url}] is not a valid absolute URL.", nameof(url));

            if (requestData == null)
                throw new ArgumentNullException(nameof(requestData));

            var httpResponse = await httpRetryPolicy.Execute(async () =>
            {
                try
                {
                    return EnsureSuccessStatusCode(await httpClient.PutAsync(url, ToHttpContent(requestData)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    throw;
                }
            });

            return new HttpResponse<TResponse>(httpResponse.StatusCode, await TryToGetResponseData<TResponse>(httpResponse));
        }

        private HttpContent ToHttpContent(object httpContentData) =>
            new StringContent(JsonConvert.SerializeObject(httpContentData), Encoding.UTF8, JsonMediaType);

        private async Task<TResponse> TryToGetResponseData<TResponse>(HttpResponseMessage httpResponse)
        {
            try
            {
                var responseText = await httpResponse.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<TResponse>(responseText);
            }
            catch
            {
                return default;
            }
        }

        private HttpResponseMessage EnsureSuccessStatusCode(HttpResponseMessage httpResponse)
        {
            if (httpResponse.StatusCode != HttpStatusCode.BadRequest)
            {
                httpResponse.EnsureSuccessStatusCode();
            }

            return httpResponse;
        }
    }
}
