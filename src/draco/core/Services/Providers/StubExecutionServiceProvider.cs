// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.Core.Services.Interfaces;
using Draco.Core.Services.Providers;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Core.Services.Providers
{
    public class StubExecutionServiceProvider : BaseExecutionServiceProvider, IExecutionServiceProvider // stub/v1: echoes back execution request.
    {
        public override Task<JObject> GetServiceConfigurationAsync(ExecutionRequest execRequest)
        {
            if (execRequest == null)
            {
                throw new ArgumentNullException(nameof(execRequest));
            }

            return Task.FromResult(JObject.FromObject(execRequest));
        }
    }
}
