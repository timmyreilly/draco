// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;

namespace Draco.Core.Execution.Interfaces
{
    public interface IExecutionRequestSubscriber
    {
        Task SubscribeAsync(CancellationToken cancelToken);

        bool IsSubscribed { get; }
    }
}
