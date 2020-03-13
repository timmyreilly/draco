// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Interfaces;
using Draco.Core.Models;
using Draco.Core.Services.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Draco.Core.Services.Providers
{
    public class CompositeExecutionServiceProvider : IExecutionServiceProvider
    {
        private readonly INamedServiceFactory<IExecutionServiceProvider> execServiceProviderFactory;
        private readonly IServiceProvider serviceProvider;

        public CompositeExecutionServiceProvider(
            INamedServiceFactory<IExecutionServiceProvider> execServiceProviderFactory,
            IServiceProvider serviceProvider)
        {
            this.execServiceProviderFactory = execServiceProviderFactory ?? 
                                              throw new ArgumentNullException(nameof(execServiceProviderFactory));

            this.serviceProvider = serviceProvider ?? 
                                   throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<JObject> GetServiceConfigurationAsync(ExecutionRequest executionRequest)
        {
            if (executionRequest == null)
            {
                throw new ArgumentNullException(nameof(executionRequest));
            }

            var configDictionary = new Dictionary<string, JObject>();

            foreach (var serviceName in executionRequest.SupportedServices.Keys.Intersect(execServiceProviderFactory.Keys))
            {
                var execServiceProvider = execServiceProviderFactory.CreateService(serviceName, serviceProvider);
                var execServiceConfig = await execServiceProvider.GetServiceConfigurationAsync(executionRequest);

                if (execServiceConfig != null)
                {
                    configDictionary.Add(serviceName, execServiceConfig);
                }
            }

            return JObject.FromObject(configDictionary);
        }

        public Task OnExecutedAsync(ExecutionContext execContext) =>
            On(execContext, (esp, ec) => esp.OnExecutedAsync(ec));

        public Task OnExecutingAsync(ExecutionRequest execRequest) =>
            On(execRequest, (esp, er) => esp.OnExecutingAsync(er));

        public Task OnValidatedAsync(ExecutionContext execContext) =>
            On(execContext, (esp, ec) => esp.OnValidatedAsync(ec));

        public Task OnValidatingAsync(ExecutionRequest execRequest) =>
            On(execRequest, (esp, er) => esp.OnValidatingAsync(er));

        private Task On(ExecutionRequest execRequest, Func<IExecutionServiceProvider, ExecutionRequest, Task> onFunc)
        {
            if (execRequest == null)
            {
                throw new ArgumentNullException(nameof(execRequest));
            }

            Task.WaitAll(execRequest.SupportedServices.Keys
                                    .Intersect(execServiceProviderFactory.Keys)
                                    .Select(sn => onFunc(execServiceProviderFactory.CreateService(sn, serviceProvider), execRequest))
                                    .ToArray());

            return Task.CompletedTask;
        }

        private Task On(ExecutionContext execContext, Func<IExecutionServiceProvider, ExecutionContext, Task> onFunc)
        {
            if (execContext == null)
            {
                throw new ArgumentNullException(nameof(execContext));
            }

            Task.WaitAll(execContext.SupportedServices.Keys
                                    .Intersect(execServiceProviderFactory.Keys)
                                    .Select(sn => onFunc(execServiceProviderFactory.CreateService(sn, serviceProvider), execContext))
                                    .ToArray());

            return Task.CompletedTask;
        }
    }
}
