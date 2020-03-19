// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.ObjectStorage.Models;
using System.Collections.Generic;

namespace Draco.Api.InternalModels.Extensions
{
    /// <summary>
    /// Extension methods for validating input object accessor request API models and
    /// converting to/from input object accessor request API/core models
    /// </summary>
    public static class InputObjectAccessorRequestExtensions
    {
        /// <summary>
        /// Converts an input object accessor request API model to a core model
        /// </summary>
        /// <param name="apiModel"></param>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static InputObjectAccessorRequest ToCoreModel(this InputObjectAccessorRequestApiModel apiModel, string objectName) =>
            new InputObjectAccessorRequest
            {
                ExecutionMetadata = apiModel.ExecutionMetadata.ToCoreModel(),
                ExpirationPeriod = apiModel.ExpirationPeriod,
                ObjectMetadata = apiModel.ObjectMetadata.ToCoreModel(objectName),
                ObjectProviderName = apiModel.ObjectProviderName,
                SignatureRsaKeyXml = apiModel.SignatureRsaKeyXml
            };

        /// <summary>
        /// Converts an input object accessor request core model to an API model
        /// </summary>
        /// <param name="coreModel"></param>
        /// <returns></returns>
        public static InputObjectAccessorRequestApiModel ToApiModel(this InputObjectAccessorRequest coreModel) =>
            new InputObjectAccessorRequestApiModel
            {
                ExecutionMetadata = coreModel.ExecutionMetadata.ToApiModel(),
                ExpirationPeriod = coreModel.ExpirationPeriod,
                ObjectMetadata = coreModel.ObjectMetadata.ToApiModel(),
                ObjectProviderName = coreModel.ObjectProviderName,
                SignatureRsaKeyXml = coreModel.SignatureRsaKeyXml
            };

        /// <summary>
        /// Validates an input object accessor request API model
        /// </summary>
        /// <param name="apiModel"></param>
        /// <returns></returns>
        public static IEnumerable<string> ValidateApiModel(this InputObjectAccessorRequestApiModel apiModel)
        {
            if (string.IsNullOrEmpty(apiModel.ObjectName))
            {
                yield return "[objectName] is required.";
            }

            if (string.IsNullOrEmpty(apiModel.ObjectProviderName))
            {
                yield return "[objectProviderName] is required.";
            }

            if (string.IsNullOrEmpty(apiModel.SignatureRsaKeyXml))
            {
                yield return "[signatureRsaKeyXml] is required.";
            }

            if (apiModel.ObjectMetadata == null)
            {
                yield return "[objectMetadata] is required.";
            }

            if (apiModel.ExecutionMetadata == null)
            {
                yield return "[executionMetadata] is required.";
            }
            else
            {
                foreach (var emError in apiModel.ExecutionMetadata.ValidateApiModel())
                {
                    yield return $"[executionMetadata]: {emError}";
                }
            }
        }
    }
}
