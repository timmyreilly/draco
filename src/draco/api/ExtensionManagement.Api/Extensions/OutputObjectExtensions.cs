// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.ExtensionManagement.Api.Models;

namespace Draco.ExtensionManagement.Api.Extensions
{
    /// <summary>
    /// Extension methods used to convert to/from output object API/core models
    /// </summary>
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
