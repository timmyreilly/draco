// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.ExtensionManagement.Api.Models;

namespace Draco.ExtensionManagement.Api.Extensions
{
    public static class InputObjectExtensions
    {
        public static ExtensionInputObject ToCoreModel(this InputObjectApiModel apiModel) =>
            new ExtensionInputObject
            {
                Description = apiModel.Description,
                IsRequired = apiModel.IsRequired,
                Name = apiModel.Name.ToLower(),
                ObjectTypeName = apiModel.ObjectTypeName,
                ObjectTypeUrl = apiModel.ObjectTypeUrl
            };

        public static InputObjectApiModel ToApiModel(this ExtensionInputObject coreModel, string extensionId, string exVersionId) =>
            new InputObjectApiModel
            {
                Description = coreModel.Description,
                ExtensionId = extensionId,
                ExtensionVersionId = exVersionId,
                IsRequired = coreModel.IsRequired,
                Name = coreModel.Name,
                ObjectTypeName = coreModel.ObjectTypeName,
                ObjectTypeUrl = coreModel.ObjectTypeUrl
            };
    }
}
