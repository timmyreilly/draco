// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Core.Execution.Interfaces
{
    /// <summary>
    /// Defines a mechanism for dispatching asynchronous execution requests, typically to a message broker like Azure service bus,
    /// for later processing. For more information on the execution pipeline, see /doc/architecture/execution-pipeline.md.
    /// </summary>
    public interface IAsyncExecutionDispatcher : IExecutionAdapter
    {
    }
}
