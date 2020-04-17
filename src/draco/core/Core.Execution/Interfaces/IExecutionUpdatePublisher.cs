// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Draco.Core.Execution.Interfaces
{
    /// <summary>
    /// Defines a mechanism for publishing execution updates to some external location (by default Azure Event Grid).
    /// For more information on execution updates and events, see /doc#execution-events.
    /// </summary>
    public interface IExecutionUpdatePublisher
    {
        Task PublishUpdateAsync(Core.Models.Execution execution);
    }
}
