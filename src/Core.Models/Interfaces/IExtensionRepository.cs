using System.Threading.Tasks;

namespace Draco.Core.Models.Interfaces
{
    public interface IExtensionRepository
    {
        Task<Extension> GetExtensionAsync(string extensionId);
        Task<ExtensionVersion> GetExtensionVersionAsync(string extensionId, string versionId);
        Task<string> SaveExtensionAsync(Extension extension);
        Task<string> SaveExtensionVersionAsync(ExtensionVersion extensionVersion);
    }
}
