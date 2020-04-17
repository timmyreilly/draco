// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Draco.Core.Execution.Interfaces
{
    /// <summary>
    /// Defines a mechanism for routing execution requests to the appropriate processor.
    /// For more information on the role that the router plays in the Draco execution pipeline, see /doc/architecture/execution-pipeline.md.
    /// </summary>
    public interface IExecutionRequestRouter
    {
        Task<Core.Models.ExecutionContext> RouteRequestAsync(ExecutionRequest execRequest, CancellationToken cancelToken);
    }
}
