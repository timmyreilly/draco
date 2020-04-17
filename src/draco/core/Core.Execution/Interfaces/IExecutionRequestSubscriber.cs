// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;

namespace Draco.Core.Execution.Interfaces
{
    /// <summary>
    /// Defines a mechanism for subscribing to asynchronous execution requests. Out-of-the-box, this interface is
    /// used by the remote execution agent to listen for and process remote executions requests.
    /// </summary>
    public interface IExecutionRequestSubscriber
    {
        Task SubscribeAsync(CancellationToken cancelToken);

        bool IsSubscribed { get; }
    }
}
