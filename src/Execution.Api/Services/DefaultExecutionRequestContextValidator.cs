using Draco.Execution.Api.Interfaces;
using System.Threading.Tasks;

namespace Draco.Execution.Api.Services
{
    public class DefaultExecutionRequestContextValidator : IExecutionRequestContextValidator
    {
        public Task ValidateExecutionRequestContextAsync(IExecutionRequestContext erContext)
        {
            return Task.CompletedTask;
        }
    }
}
