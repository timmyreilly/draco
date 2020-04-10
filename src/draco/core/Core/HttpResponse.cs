// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;

namespace Draco.Core
{
    /// <summary>
    /// Describes a generic HTTP response returned by a REST API.
    /// </summary>
    public class HttpResponse
    {
        public HttpResponse() { }

        public HttpResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; set; }
    }

    /// <summary>
    /// Describes a generic HTTP response and type-specific body returned by a REST API.
    /// </summary>
    /// <typeparam name="TResponse">The type of HTTP response body</typeparam>
    public class HttpResponse<TResponse> : HttpResponse
    {
        public HttpResponse() { }

        public HttpResponse(HttpStatusCode statusCode, TResponse content)
            : base(statusCode)
        {
            Content = content;
        }

        public TResponse Content { get; set; }
    }
     
}
