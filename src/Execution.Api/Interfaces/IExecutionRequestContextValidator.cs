using Draco.Execution.Api.Models;
using System.Threading.Tasks;

namespace Draco.Execution.Api.Interfaces
{
    public interface IExecutionRequestContextValidator
    {
        Task ValidateExecutionRequestContextAsync(IExecutionRequestContext erContext);
    }
}
