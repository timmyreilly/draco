// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using System.Linq;

namespace Draco.Api.InternalModels.Extensions
{
    public static class ExecutionResponseExtensions
    {
        public static ExecutionContext ToCoreModel(ExecutionResponseApiModel apiModel)
        {
            var coreModel = new ExecutionContext
            {
                CreatedDateTimeUtc = apiModel.CreatedDateTimeUtc,
                ExecutionId = apiModel.ExecutionId,
                ExecutionModelName = apiModel.ExecutionModelName,
                ExecutionProfileName = apiModel.ExecutionProfileName,
                ExecutionTimeoutDateTimeUtc = apiModel.ExecutionTimeoutDateTimeUtc,
                Executor = apiModel.Executor.ToCoreModel(),
                ExecutorProperties = apiModel.ExecutorProperties,
                ExtensionId = apiModel.ExtensionId,
                ExtensionVersionId = apiModel.ExtensionVersionId,
                LastUpdatedDateTimeUtc = apiModel.LastUpdatedDateTimeUtc,
                ObjectProviderName = apiModel.ObjectProviderName,
                PercentComplete = apiModel.PercentComplete,
                Priority = apiModel.Priority,
                ResultData = apiModel.ResultData,
                Status = apiModel.Status,
                StatusMessage = apiModel.StatusMessage,
                StatusUpdateKey = apiModel.StatusUpdateKey,
                SupportedServices = apiModel.SupportedServices,
                ValidationErrors = apiModel.ValidationErrors.Select(e => e.ToCoreModel()).ToList()
            };

            ApplyInputObjectsToCoreModel(coreModel, apiModel);
            ApplyOutputObjectsToCoreModel(coreModel, apiModel);

            return coreModel;
        }

        public static ExecutionResponseApiModel ToApiModel(ExecutionContext coreModel)
        {
            var apiModel = new ExecutionResponseApiModel
            {
                CreatedDateTimeUtc = coreModel.CreatedDateTimeUtc,
                ExecutionId = coreModel.ExecutionId,
                ExecutionModelName = coreModel.ExecutionModelName,
                ExecutionProfileName = coreModel.ExecutionProfileName,
                ExecutionTimeoutDateTimeUtc = coreModel.ExecutionTimeoutDateTimeUtc,
                Executor = coreModel.Executor.ToApiModel(),
                ExecutorProperties = coreModel.ExecutorProperties,
                ExtensionId = coreModel.ExtensionId,
                ExtensionVersionId = coreModel.ExtensionVersionId,
                LastUpdatedDateTimeUtc = coreModel.LastUpdatedDateTimeUtc,
                ObjectProviderName = coreModel.ObjectProviderName,
                PercentComplete = coreModel.PercentComplete,
                Priority = coreModel.Priority,
                ResultData = coreModel.ResultData,
                Status = coreModel.Status,
                StatusMessage = coreModel.StatusMessage,
                StatusUpdateKey =  coreModel.StatusUpdateKey,
                SupportedServices = coreModel.SupportedServices,
                ValidationErrors = coreModel.ValidationErrors.Select(e => e.ToApiModel()).ToList()
            };

            ApplyInputObjectsToApiModel(coreModel, apiModel);
            ApplyOutputObjectsToApiModel(coreModel, apiModel);

            return apiModel;
        }

        private static ExecutionResponseApiModel ApplyInputObjectsToApiModel(ExecutionContext coreModel, ExecutionResponseApiModel apiModel)
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

        private static ExecutionResponseApiModel ApplyOutputObjectsToApiModel(ExecutionContext coreModel, ExecutionResponseApiModel apiModel)
        {
            foreach (var objectName in coreModel.OutputObjects.Keys)
            {
                var ooCoreModel = coreModel.OutputObjects[objectName];
                var ooApiModel = ooCoreModel.ToApiModel();

                ooApiModel.IsProvided = coreModel.ProvidedOutputObjects.Contains(objectName);

                apiModel.OutputObjects.Add(objectName, ooApiModel);
            }

            return apiModel;
        }

        private static ExecutionContext ApplyInputObjectsToCoreModel(ExecutionContext coreModel, ExecutionResponseApiModel apiModel)
        {
            foreach (var objectName in apiModel.InputObjects.Keys)
            {
                var ioApiModel = apiModel.InputObjects[objectName];
                var ioCoreModel = ioApiModel.ToCoreModel(objectName);

                coreModel.InputObjects.Add(objectName, ioCoreModel);

                if (ioApiModel.IsProvided)
                {
                    coreModel.ProvidedInputObjects.Add(objectName);
                }
            }

            return coreModel;
        }

        private static ExecutionContext ApplyOutputObjectsToCoreModel(ExecutionContext coreModel, ExecutionResponseApiModel apiModel)
        {
            foreach (var objectName in apiModel.OutputObjects.Keys)
            {
                var ooApiModel = apiModel.OutputObjects[objectName];
                var ooCoreModel = ooApiModel.ToCoreModel(objectName);

                coreModel.OutputObjects.Add(objectName, ooCoreModel);

                if (ooApiModel.IsProvided)
                {
                    coreModel.ProvidedOutputObjects.Add(objectName);
                }
            }

            return coreModel;
        }
    }
}
