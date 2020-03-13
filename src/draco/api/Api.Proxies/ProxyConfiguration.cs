// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Api.Proxies
{
    public class ProxyConfiguration
    {
        public ProxyConfiguration() { }

        public ProxyConfiguration(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; set; }
    }
}
