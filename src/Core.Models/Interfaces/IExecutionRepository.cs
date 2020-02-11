using System.Collections.Generic;
using System.Threading.Tasks;

namespace Draco.Core.Models.Interfaces
{
    public interface IExecutionRepository
    {
        Task<Execution> GetExecutionAsync(string executionId, string tenantId);
        Task<IEnumerable<Execution>> GetExecutionsByUserAsync(string userId, string tenantId);
        Task<IEnumerable<Execution>> GetExecutionsByTenantAsync(string tenantId);
        Task UpsertExecutionAsync(Execution execution);
    }
}
