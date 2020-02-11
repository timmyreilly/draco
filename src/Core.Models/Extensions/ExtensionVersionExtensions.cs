using System.Linq;

namespace Draco.Core.Models.Extensions
{
    public static class ExtensionVersionExtensions
    {
        public static ExecutionProfile GetExecutionProfile(this ExtensionVersion exVersion, string profileName) =>
            exVersion.ExecutionProfiles.SingleOrDefault(ep => (ep.ProfileName == profileName));

        public static ExtensionInputObject GetInputObject(this ExtensionVersion exVersion, string objectName) =>
            exVersion.InputObjects.SingleOrDefault(io => (io.Name == objectName));

        public static ExtensionOutputObject GetOutputObject(this ExtensionVersion exVersion, string objectName) =>
            exVersion.OutputObjects.SingleOrDefault(oo => (oo.Name == objectName));
    }
}
