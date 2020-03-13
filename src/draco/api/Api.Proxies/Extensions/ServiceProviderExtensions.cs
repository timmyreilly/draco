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
    public static class ServiceProviderExtensions
    {
        public static IExecutionServiceProvider GetProxyExecutionServiceProvider(this IServiceProvider serviceProvider, string baseUrl)
        {
            ValidateArguments(serviceProvider, baseUrl);

            return new ProxyExecutionServiceProvider(serviceProvider.GetService<IJsonHttpClient>(), new ProxyConfiguration(baseUrl));
        }

        public static IExecutionAdapter GetProxyExecutionAdapter(this IServiceProvider serviceProvider, string baseUrl)
        {
            ValidateArguments(serviceProvider, baseUrl);

            return new ProxyExecutionAdapter(serviceProvider.GetService<IJsonHttpClient>(), new ProxyConfiguration(baseUrl));
        }

        public static IInputObjectAccessorProvider GetProxyInputObjectAccessorProvider(this IServiceProvider serviceProvider, string baseUrl)
        {
            ValidateArguments(serviceProvider, baseUrl);

            return new ProxyInputObjectAccessorProvider(serviceProvider.GetService<IJsonHttpClient>(), new ProxyConfiguration(baseUrl));
        }

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
