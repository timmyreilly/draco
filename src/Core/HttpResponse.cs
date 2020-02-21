// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net;

namespace Draco.Core
{
    public class HttpResponse
    {
        public HttpResponse() { }

        public HttpResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; set; }
    }

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
