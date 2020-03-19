// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using System.Collections.Generic;

namespace Draco.Api.InternalModels.Extensions
{
    /// <summary>
    /// Extension methods for validating input object definition API models
    /// and converting to/from input object definition API/core models
    /// </summary>
    public static class InputObjectExtensions
    {
        /// <summary>
        /// Converts an input object definition core model to an API model
        /// </summary>
        /// <param name="coreModel"></param>
        /// <returns></returns>
        public static InputObjectApiModel ToApiModel(this ExtensionInputObject coreModel) =>
            new InputObjectApiModel
            {
                Description = coreModel.Description,
                IsRequired = coreModel.IsRequired,
                ObjectTypeName = coreModel.ObjectTypeName,
                ObjectTypeUrl = coreModel.ObjectTypeUrl
            };

        /// <summary>
        /// Converts an input object definition API model to a core model
        /// </summary>
        /// <param name="apiModel"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static ExtensionInputObject ToCoreModel(this InputObjectApiModel apiModel, string objectName) =>
            new ExtensionInputObject
            {
                Description = apiModel.Description,
                IsRequired = apiModel.IsRequired,
                Name = objectName,
                ObjectTypeName = apiModel.ObjectTypeName,
                ObjectTypeUrl = apiModel.ObjectTypeUrl
            };

        /// <summary>
        /// Validates an input object definition API model
        /// </summary>
        /// <param name="apiModel"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static IEnumerable<string> ValidateApiModel(this InputObjectApiModel apiModel, string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
            {
                yield return "[name] is required.";
            }
        }
    }
}
