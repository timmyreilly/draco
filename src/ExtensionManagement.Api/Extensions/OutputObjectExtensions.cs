using Draco.Core.Models;
using Draco.ExtensionManagement.Api.Models;

namespace Draco.ExtensionManagement.Api.Extensions
{
    public static class OutputObjectExtensions
    {
        public static ExtensionOutputObject ToCoreModel(this OutputObjectApiModel apiModel) =>
            new ExtensionOutputObject
            {
                Description = apiModel.Description,
                Name = apiModel.Name.ToLower(),
                ObjectTypeName = apiModel.ObjectTypeName,
                ObjectTypeUrl = apiModel.ObjectTypeUrl 
            };

        public static OutputObjectApiModel ToApiModel(this ExtensionOutputObject coreModel, string extensionId, string exVersionId) =>
            new OutputObjectApiModel
            {
                Description = coreModel.Description,
                ExtensionId = extensionId,
                ExtensionVersionId = exVersionId,
                Name = coreModel.Name,
                ObjectTypeName = coreModel.ObjectTypeName,
                ObjectTypeUrl = coreModel.ObjectTypeUrl
            };
    }
}
