// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.ObjectStorage.Models;
using System.Collections.Generic;

namespace Draco.Api.InternalModels.Extensions
{
    public static class InputObjectAccessorRequestExtensions
    {
        public static InputObjectAccessorRequest ToCoreModel(this InputObjectAccessorRequestApiModel apiModel, string objectName) =>
            new InputObjectAccessorRequest
            {
                ExecutionMetadata = apiModel.ExecutionMetadata.ToCoreModel(),
                ExpirationPeriod = apiModel.ExpirationPeriod,
                ObjectMetadata = apiModel.ObjectMetadata.ToCoreModel(objectName),
                ObjectProviderName = apiModel.ObjectProviderName,
                SignatureRsaKeyXml = apiModel.SignatureRsaKeyXml
            };

        public static InputObjectAccessorRequestApiModel ToApiModel(this InputObjectAccessorRequest coreModel) =>
            new InputObjectAccessorRequestApiModel
            {
                ExecutionMetadata = coreModel.ExecutionMetadata.ToApiModel(),
                ExpirationPeriod = coreModel.ExpirationPeriod,
                ObjectMetadata = coreModel.ObjectMetadata.ToApiModel(),
                ObjectProviderName = coreModel.ObjectProviderName,
                SignatureRsaKeyXml = coreModel.SignatureRsaKeyXml
            };

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
