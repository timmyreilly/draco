using Draco.Core.Models;
using System.Collections.Generic;

namespace Draco.Api.InternalModels.Extensions
{
    public static class OutputObjectExtensions
    {
        public static OutputObjectApiModel ToApiModel(this ExtensionOutputObject coreModel) =>
            new OutputObjectApiModel
            {
                Description = coreModel.Description,
                ObjectTypeName = coreModel.ObjectTypeName,
                ObjectTypeUrl = coreModel.ObjectTypeUrl
            };

        public static ExtensionOutputObject ToCoreModel(this OutputObjectApiModel apiModel, string objectName) =>
            new ExtensionOutputObject
            {
                Description = apiModel.Description,
                Name = objectName,
                ObjectTypeName = apiModel.ObjectTypeName,
                ObjectTypeUrl = apiModel.ObjectTypeUrl
            };

        public static IEnumerable<string> ValidateApiModel(this OutputObjectApiModel apiModel, string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
            {
                yield return "[name] is required.";
            }
        }
    }
}
