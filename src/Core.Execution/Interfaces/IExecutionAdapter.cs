using Draco.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Draco.Core.Execution.Interfaces
{
    public interface IExecutionAdapter
    {
        Task<Core.Models.ExecutionContext> ExecuteAsync(ExecutionRequest request, CancellationToken cancelToken);
    }
}
