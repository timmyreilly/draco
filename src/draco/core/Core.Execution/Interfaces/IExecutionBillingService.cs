// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using System.Threading.Tasks;

namespace Draco.Core.Execution.Interfaces
{
    public interface IExecutionBillingService
    {
        Task OnExecutionUpdatedAsync(ExecutionUpdate execUpdate);
        Task<bool> PrequalifyExecutionAsync(ExecutionRequest execRequest);
    }
}
