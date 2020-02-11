using Draco.Core.Execution.Interfaces;
using Draco.Core.Models;
using System;
using System.Threading.Tasks;

namespace Draco.Core.Execution.Services
{
    public class DefaultExecutionBillingService : IExecutionBillingService
    {
        public Task OnExecutionUpdatedAsync(ExecutionUpdate execUpdate)
        {
            if (execUpdate == null)
                throw new ArgumentNullException(nameof(execUpdate));

            return Task.CompletedTask;
        }

        public Task<bool> PrequalifyExecutionAsync(ExecutionRequest execReqeust) => Task.FromResult(true);
    }
}
