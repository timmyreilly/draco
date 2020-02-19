// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.Core.Models.Enumerations;
using System.Collections.Generic;

namespace Draco.Api.InternalModels.Extensions
{
    public static class ExecutionRequestExtensions
    {
        public static ExecutionRequest ToCoreModel(this ExecutionRequestApiModel apiModel)
        {
            var coreModel = new ExecutionRequest
            {
                CreatedDateTimeUtc = apiModel.CreatedDateTimeUtc,
                ExecutionId = apiModel.ExecutionId,
                ExecutionModelName = apiModel.ExecutionModelName,
                ExecutionParameters = apiModel.ExecutionParameters,
                ExecutionProfileName = apiModel.ExecutionProfileName,
                ExecutionTimeoutDateTimeUtc = apiModel.ExecutionTimeoutDateTimeUtc,
                ExecutionTimeoutDuration = apiModel.ExecutionTimeoutDuration,
                Executor = apiModel.Executor.ToCoreModel(),
                ExecutorProperties = apiModel.ExecutorProperties,
                ExtensionId = apiModel.ExtensionId,
                ExtensionSettings = apiModel.ExtensionSettings,
                ExtensionVersionId = apiModel.ExtensionVersionId,
                GetExecutionStatusUrl = apiModel.GetExecutionStatusUrl,
                IsValidationSupported = apiModel.IsValidationSupported,
                LastUpdatedDateTimeUtc = apiModel.LastUpdatedDateTimeUtc,
                ObjectProviderName = apiModel.ObjectProviderName,
                Priority = apiModel.Priority,
                SignatureRsaKeyXml = apiModel.SignatureRsaKeyXml,
                StatusUpdateKey = apiModel.StatusUpdateKey,
                SupportedServices = apiModel.SupportedServices,
                UpdateExecutionStatusUrl = apiModel.PutExecutionStatusUrl,
                ValidateOnly = apiModel.ValidateOnly
            };

            ApplyInputObjectsToCoreModel(coreModel, apiModel);
            ApplyOutputObjectsToCoreModel(coreModel, apiModel);

            return coreModel;
        }

        public static ExecutionRequestApiModel ToApiModel(this ExecutionRequest coreModel)
        {
            var apiModel = new ExecutionRequestApiModel
            {
                CreatedDateTimeUtc = coreModel.CreatedDateTimeUtc,
                ExecutionId = coreModel.ExecutionId,
                ExecutionModelName = coreModel.ExecutionModelName,
                ExecutionParameters = coreModel.ExecutionParameters,
                ExecutionProfileName = coreModel.ExecutionProfileName,
                ExecutionTimeoutDateTimeUtc = coreModel.ExecutionTimeoutDateTimeUtc,
                ExecutionTimeoutDuration = coreModel.ExecutionTimeoutDuration,
                Executor = coreModel.Executor.ToApiModel(),
                ExecutorProperties = coreModel.ExecutorProperties,
                ExtensionId = coreModel.ExtensionId,
                ExtensionSettings = coreModel.ExtensionSettings,
                ExtensionVersionId = coreModel.ExtensionVersionId,
                GetExecutionStatusUrl = coreModel.GetExecutionStatusUrl,
                IsValidationSupported = coreModel.IsValidationSupported,
                LastUpdatedDateTimeUtc = coreModel.LastUpdatedDateTimeUtc,
                ObjectProviderName = coreModel.ObjectProviderName,
                Priority = coreModel.Priority,
                PutExecutionStatusUrl = coreModel.UpdateExecutionStatusUrl,
                SignatureRsaKeyXml = coreModel.SignatureRsaKeyXml,
                StatusUpdateKey = coreModel.StatusUpdateKey,
                SupportedServices = coreModel.SupportedServices,
                ValidateOnly = coreModel.ValidateOnly
            };

            ApplyInputObjectsToApiModel(coreModel, apiModel);
            ApplyOutputObjectsToApiModel(coreModel, apiModel);

            return apiModel;
        }

        public static IEnumerable<string> ValidateApiModel(this ExecutionRequestApiModel apiModel)
        {
            if (string.IsNullOrEmpty(apiModel.ExecutionId))
            {
                yield return "[executionId] is required.";
            }

            if (string.IsNullOrEmpty(apiModel.ExtensionId))
            {
                yield return "[extensionId] is required.";
            }

            if (string.IsNullOrEmpty(apiModel.ExtensionVersionId))
            {
                yield return "[extensionVersionId] is required.";
            }

            if (string.IsNullOrEmpty(apiModel.ExecutionModelName))
            {
                yield return "[executionModelName] is required.";
            }

            if (string.IsNullOrEmpty(apiModel.ObjectProviderName))
            {
                yield return "[objectProviderName] is required.";
            }

            if (string.IsNullOrEmpty(apiModel.StatusUpdateKey))
            {
                yield return "[statusUpdateKey] is required.";
            }

            if (string.IsNullOrEmpty(apiModel.GetExecutionStatusUrl))
            {
                yield return "[getExecutionStatusUrl] is required.";
            }

            if (string.IsNullOrEmpty(apiModel.PutExecutionStatusUrl))
            {
                yield return "[putExecutionStatusUrl] is required.";
            }

            if (string.IsNullOrEmpty(apiModel.SignatureRsaKeyXml))
            {
                yield return "[signatureRsaKeyXml] is required.";
            }

            if (apiModel.CreatedDateTimeUtc == default)
            {
                yield return "[createdDateTimeUtc] is required.";
            }

            if (apiModel.LastUpdatedDateTimeUtc == default)
            {
                yield return "[lastUpdatedDateTimeUtc] is required.";
            }

            if (apiModel.Executor == null)
            {
                yield return "[executor] is required.";
            }
            else
            {
                foreach (var executorError in apiModel.Executor.ValidateApiModel())
                {
                    yield return $"[executor]: {executorError}";
                }
            }

            if (apiModel.Priority == ExecutionPriority.Undefined)
            {
                yield return "[priority] is required; valid priorities are [1] (low), [2] (normal), and [3] (high).";
            }
        }

        private static ExecutionRequestApiModel ApplyInputObjectsToApiModel(ExecutionRequest coreModel, ExecutionRequestApiModel apiModel)
        {
            foreach (var objectName in coreModel.InputObjects.Keys)
            {
                var ioCoreModel = coreModel.InputObjects[objectName];
                var ioApiModel = ioCoreModel.ToApiModel();

                ioApiModel.IsProvided = coreModel.ProvidedInputObjects.Contains(objectName);

                apiModel.InputObjects.Add(objectName, ioApiModel);
            }

            return apiModel;
        }

        private static ExecutionRequestApiModel ApplyOutputObjectsToApiModel(ExecutionRequest coreModel, ExecutionRequestApiModel apiModel)
        {
            foreach (var objectName in coreModel.OutputObjects.Keys)
            {
                var ooCoreModel = coreModel.OutputObjects[objectName];

                apiModel.OutputObjects.Add(objectName, ooCoreModel.ToApiModel());
            }

            return apiModel;
        }

        private static ExecutionRequest ApplyInputObjectsToCoreModel(ExecutionRequest coreModel, ExecutionRequestApiModel apiModel)
        {
            foreach (var objectName in apiModel.InputObjects.Keys)
            {
                var ioApiModel = apiModel.InputObjects[objectName];

                coreModel.InputObjects.Add(objectName, ioApiModel.ToCoreModel(objectName));
                
                if (ioApiModel.IsProvided)
                {
                    coreModel.ProvidedInputObjects.Add(objectName);
                }
            }

            return coreModel;
        }

        private static ExecutionRequest ApplyOutputObjectsToCoreModel(ExecutionRequest coreModel, ExecutionRequestApiModel apiModel)
        {
            foreach (var objectName in apiModel.OutputObjects.Keys)
            {
                var ooApiModel = apiModel.OutputObjects[objectName];

                coreModel.OutputObjects.Add(objectName, ooApiModel.ToCoreModel(objectName));
            }

            return coreModel;
        }
    }
}
