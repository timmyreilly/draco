// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Draco.Core.Execution.Interfaces
{
    /// <summary>
    /// Defines a thin, execution model-specific mechanism for executing an extension.
    /// For more information on execution models, see /doc/architecture/execution-models.md.
    /// The role of the execution adapter in the context of the Draco execution pipeline is explained in detail at /doc/architecture/execution-pipeline.md.
    /// </summary>
    public interface IExecutionAdapter
    {
        Task<Core.Models.ExecutionContext> ExecuteAsync(ExecutionRequest request, CancellationToken cancelToken);
    }
}
