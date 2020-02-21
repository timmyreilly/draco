// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.Core.Services.Interfaces;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Draco.Core.Services.Providers
{
    public abstract class BaseExecutionServiceProvider : IExecutionServiceProvider
    {
        public abstract Task<JObject> GetServiceConfigurationAsync(ExecutionRequest execRequest);

        public virtual Task OnExecutedAsync(ExecutionContext execContext) => Task.CompletedTask;

        public virtual Task OnExecutingAsync(ExecutionRequest execRequest) => Task.CompletedTask;

        public virtual Task OnValidatedAsync(ExecutionContext execRequest) => Task.CompletedTask;

        public virtual Task OnValidatingAsync(ExecutionRequest execRequest) => Task.CompletedTask;
    }
}
