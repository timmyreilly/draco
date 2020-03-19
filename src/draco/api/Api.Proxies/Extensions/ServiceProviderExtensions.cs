// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Interfaces;
using Draco.Core.Interfaces;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Draco.Api.Proxies.Extensions
{
    /// <summary>
    /// Extension methods for creating internal API proxies
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Creates an internal execution service provider API proxy
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public static IExecutionServiceProvider GetProxyExecutionServiceProvider(this IServiceProvider serviceProvider, string baseUrl)
        {
            ValidateArguments(serviceProvider, baseUrl);

            return new ProxyExecutionServiceProvider(serviceProvider.GetService<IJsonHttpClient>(), new ProxyConfiguration(baseUrl));
        }

        /// <summary>
        /// Creates an internal execution adapter API proxy
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public static IExecutionAdapter GetProxyExecutionAdapter(this IServiceProvider serviceProvider, string baseUrl)
        {
            ValidateArguments(serviceProvider, baseUrl);

            return new ProxyExecutionAdapter(serviceProvider.GetService<IJsonHttpClient>(), new ProxyConfiguration(baseUrl));
        }

        /// <summary>
        /// Creates an internal input object accessor provider API proxy
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public static IInputObjectAccessorProvider GetProxyInputObjectAccessorProvider(this IServiceProvider serviceProvider, string baseUrl)
        {
            ValidateArguments(serviceProvider, baseUrl);

            return new ProxyInputObjectAccessorProvider(serviceProvider.GetService<IJsonHttpClient>(), new ProxyConfiguration(baseUrl));
        }

        /// <summary>
        /// Creates an internal output object accessor provider API proxy
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public static IOutputObjectAccessorProvider GetProxyOutputObjectAccessorProvider(this IServiceProvider serviceProvider, string baseUrl)
        {
            ValidateArguments(serviceProvider, baseUrl);

            return new ProxyOutputObjectAccessorProvider(serviceProvider.GetService<IJsonHttpClient>(), new ProxyConfiguration(baseUrl));
        }

        private static void ValidateArguments(IServiceProvider serviceProvider, string baseUrl)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }

            if (Uri.TryCreate(baseUrl, UriKind.Absolute, out _) == false)
            {
                throw new ArgumentException($"[baseUrl] [{baseUrl}] is invalid; [baseUrl] must be an absolute URL.");
            }
        }
    }
}
