// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Api.Proxies
{
    /// <summary>
    /// Shared configuration object needed to create internal API proxies.
    /// </summary>
    public class ProxyConfiguration
    {
        public ProxyConfiguration() { }

        public ProxyConfiguration(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        /// <summary>
        /// Base URL of internal API endpoint. Trailing URL slash optional.
        /// </summary>
        /// <value></value>
        public string BaseUrl { get; set; }
    }
}
