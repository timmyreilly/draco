// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Execution.Api.Models;
using System.Threading.Tasks;

namespace Draco.Execution.Api.Interfaces
{
    /// <summary>
    /// This interface takes in an API execution request and collects all the information needed to process it (extension, extension version, and execution profile) 
    /// into a handy execution request context (IExecutionRequestContext) which is, in turn, used by the execution API to process the request.
    /// In the process, the execution request context builder also validates the API execution request against the extension, extension version, and execution profile.
    /// </summary>
    public interface IExecutionRequestContextBuilder
    {
        public Task<ExecutionRequestContext<ExecutionRequestApiModel>> BuildExecutionRequestContextAsync(ExecutionRequestApiModel apiExecRequest);
    }
}
