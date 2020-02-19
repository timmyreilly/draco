// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using System.Collections.Generic;

namespace Draco.Api.InternalModels.Extensions
{
    public static class InputObjectExtensions
    {
        public static InputObjectApiModel ToApiModel(this ExtensionInputObject coreModel) =>
            new InputObjectApiModel
            {
                Description = coreModel.Description,
                IsRequired = coreModel.IsRequired,
                ObjectTypeName = coreModel.ObjectTypeName,
                ObjectTypeUrl = coreModel.ObjectTypeUrl
            };

        public static ExtensionInputObject ToCoreModel(this InputObjectApiModel apiModel, string objectName) =>
            new ExtensionInputObject
            {
                Description = apiModel.Description,
                IsRequired = apiModel.IsRequired,
                Name = objectName,
                ObjectTypeName = apiModel.ObjectTypeName,
                ObjectTypeUrl = apiModel.ObjectTypeUrl
            };

        public static IEnumerable<string> ValidateApiModel(this InputObjectApiModel apiModel, string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
            {
                yield return "[name] is required.";
            }
        }
    }
}
