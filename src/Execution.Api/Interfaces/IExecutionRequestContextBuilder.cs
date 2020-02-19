// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Execution.Api.Models;
using System.Threading.Tasks;

namespace Draco.Execution.Api.Interfaces
{
    public interface IExecutionRequestContextBuilder
    {
        public Task<ExecutionRequestContext<ExecutionRequestApiModel>> BuildExecutionRequestContextAsync(ExecutionRequestApiModel apiExecRequest);
    }
}
