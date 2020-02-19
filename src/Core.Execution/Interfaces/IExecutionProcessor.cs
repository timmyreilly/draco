// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Draco.Core.Execution.Interfaces
{
    public interface IExecutionProcessor
    {
        Task<Core.Models.ExecutionContext> ProcessRequestAsync(ExecutionRequest execRequest, CancellationToken cancelToken);
    }

    public interface IExecutionProcessor<TAdapter> : IExecutionProcessor
        where TAdapter : IExecutionAdapter
    { }
}
