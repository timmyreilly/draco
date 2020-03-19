// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using System.Collections.Generic;

namespace Draco.Api.InternalModels.Extensions
{
    /// <summary>
    /// Extension methods for validating output object definition API models and
    /// converting to/from output object definition API/core models
    /// </summary>
    public static class OutputObjectExtensions
    {
        /// <summary>
        /// Converts an output object definition core model to an API model
        /// </summary>
        /// <param name="coreModel"></param>
        /// <returns></returns>
        public static OutputObjectApiModel ToApiModel(this ExtensionOutputObject coreModel) =>
            new OutputObjectApiModel
            {
                Description = coreModel.Description,
                ObjectTypeName = coreModel.ObjectTypeName,
                ObjectTypeUrl = coreModel.ObjectTypeUrl
            };

        /// <summary>
        /// Converts an output object definition API model to a core model
        /// </summary>
        /// <param name="apiModel"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static ExtensionOutputObject ToCoreModel(this OutputObjectApiModel apiModel, string objectName) =>
            new ExtensionOutputObject
            {
                Description = apiModel.Description,
                Name = objectName,
                ObjectTypeName = apiModel.ObjectTypeName,
                ObjectTypeUrl = apiModel.ObjectTypeUrl
            };

        /// <summary>
        /// Validates an output object definition API model
        /// </summary>
        /// <param name="apiModel"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static IEnumerable<string> ValidateApiModel(this OutputObjectApiModel apiModel, string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
            {
                yield return "[name] is required.";
            }
        }
    }
}
