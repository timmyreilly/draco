using Draco.Core.ObjectStorage.Models;
using System.Threading.Tasks;

namespace Draco.Core.ObjectStorage.Interfaces
{
    public interface IObjectUrlProvider
    {
        Task<ObjectUrl> GetReadableUrlAsync(ObjectUrlRequest urlRequest);
        Task<ObjectUrl> GetWritableUrlAsync(ObjectUrlRequest urlRequest);
    }
}
