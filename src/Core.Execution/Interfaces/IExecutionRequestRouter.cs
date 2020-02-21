// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Draco.Core.Execution.Interfaces
{
    public interface IExecutionRequestRouter
    {
        Task<Core.Models.ExecutionContext> RouteRequestAsync(ExecutionRequest execRequest, CancellationToken cancelToken);
    }
}
