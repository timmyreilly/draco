using Draco.Core.ObjectStorage.Models;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Draco.Core.ObjectStorage.Interfaces
{
    public interface IInputObjectAccessorProvider
    {
        Task<JObject> GetReadableAccessorAsync(InputObjectAccessorRequest accessorRequest);
        Task<JObject> GetWritableAccessorAsync(InputObjectAccessorRequest accessorRequest);
    }
}
