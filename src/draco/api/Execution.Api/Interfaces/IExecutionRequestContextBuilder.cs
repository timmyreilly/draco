// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Execution.Api.Models;
using System.Threading.Tasks;

namespace Draco.Execution.Api.Interfaces
{
    /// <summary>
    /// This interface takes in the original execution request API model, validates it, and returns all the information needed to process it.
    /// While this could have been accomplished in the execution API controller itself, the logic needed here to pull together the right pieces
    /// of information is complex, and, as that information is pulled together, it's validated against the original request.
    /// </summary>
    public interface IExecutionRequestContextBuilder
    {
        public Task<ExecutionRequestContext<ExecutionRequestApiModel>> BuildExecutionRequestContextAsync(ExecutionRequestApiModel apiExecRequest);
    }
}
