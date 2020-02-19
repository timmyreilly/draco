// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Draco.Core.Execution.Interfaces
{
    public interface IExecutionUpdatePublisher
    {
        Task PublishUpdateAsync(Core.Models.Execution execution);
    }
}
