using Draco.Azure.ObjectStorage.Models;
using Draco.Core.ObjectStorage.Models;
using System.Threading.Tasks;

namespace Draco.Azure.ObjectStorage.Interfaces
{
    public interface IAzureObjectUrlProvider
    {
        Task<ObjectUrl> GetObjectUrlAsync(AzureObjectUrlRequest urlRequest);
    }
}
