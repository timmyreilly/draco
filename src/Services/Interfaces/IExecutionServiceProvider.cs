using Draco.Core.Models;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Draco.Core.Services.Interfaces
{
    public interface IExecutionServiceProvider
    {
        Task<JObject> GetServiceConfigurationAsync(ExecutionRequest execRequest);

        Task OnExecutingAsync(ExecutionRequest execRequest);
        Task OnExecutedAsync(ExecutionContext execContext);
        Task OnValidatingAsync(ExecutionRequest execRequest);
        Task OnValidatedAsync(ExecutionContext execRequest);
    }
}
