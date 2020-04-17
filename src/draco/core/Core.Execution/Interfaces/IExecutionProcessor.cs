// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Draco.Core.Execution.Interfaces
{
    /// <summary>
    /// Defines a mechanism for executing extensions (typically through the use of an execution adapter) and
    /// pushing execution updates back to the platform via the execution API. For more information on the execution pipeline 
    /// and the role of the execution processor, see /doc/architecture/execution-pipeline.md.
    /// </summary>
    public interface IExecutionProcessor
    {
        Task<Core.Models.ExecutionContext> ProcessRequestAsync(ExecutionRequest execRequest, CancellationToken cancelToken);
    }

    /// <summary>
    /// Defines an execution adapter-specific mechanism for executing extensions (through the use of the [TAdapter] execution adapter) and
    /// pushing execution updates back to the platform via the execution API. For more information on the execution pipeline
    /// and the role of the execution processor, see /doc/architecture/execution-pipeline.md.
    /// </summary>
    /// <typeparam name="TAdapter">The type of execution adapter that this processor relies on</typeparam>
    public interface IExecutionProcessor<TAdapter> : IExecutionProcessor
        where TAdapter : IExecutionAdapter
    { }
}
