// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using System.Linq;

namespace Draco.Api.InternalModels.Extensions
{
    public static class ExecutionContextExtensions
    {
        public static ExecutionContextApiModel ToApiModel(this ExecutionContext coreModel) =>
            new ExecutionContextApiModel
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
                InputObjects = coreModel.InputObjects.ToDictionary(io => io.Key, io => coreModel.ToInputObjectApiModel(io.Value)),
                LastUpdatedDateTimeUtc = coreModel.LastUpdatedDateTimeUtc,
                ObjectProviderName = coreModel.ObjectProviderName,
                OutputObjects = coreModel.OutputObjects.ToDictionary(oo => oo.Key, oo => coreModel.ToOutputObjectApiModel(oo.Value)),
                PercentComplete = coreModel.PercentComplete,
                Priority = coreModel.Priority,
                ResultData = coreModel.ResultData,
                Status = coreModel.Status,
                StatusMessage = coreModel.StatusMessage,
                StatusUpdateKey = coreModel.StatusUpdateKey,
                SupportedServices = coreModel.SupportedServices,
                ValidationErrors = coreModel.ValidationErrors.Select(ve => ve.ToApiModel()).ToList()
            };

        public static ExecutionContext ToCoreModel(this ExecutionContextApiModel apiModel) =>
            new ExecutionContext
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
                InputObjects = apiModel.InputObjects.ToDictionary(io => io.Key, io => io.Value.ToCoreModel(io.Key)),
                LastUpdatedDateTimeUtc = apiModel.LastUpdatedDateTimeUtc,
                ObjectProviderName = apiModel.ObjectProviderName,
                OutputObjects = apiModel.OutputObjects.ToDictionary(oo => oo.Key, oo => oo.Value.ToCoreModel(oo.Key)),
                PercentComplete = apiModel.PercentComplete,
                Priority = apiModel.Priority,
                ProvidedInputObjects = apiModel.InputObjects.Where(io => io.Value.IsProvided).Select(io => io.Key).ToList(),
                ProvidedOutputObjects = apiModel.OutputObjects.Where(oo => oo.Value.IsProvided).Select(oo => oo.Key).ToList(),
                ResultData = apiModel.ResultData,
                Status = apiModel.Status,
                StatusMessage = apiModel.StatusMessage,
                StatusUpdateKey = apiModel.StatusUpdateKey,
                SupportedServices = apiModel.SupportedServices,
                ValidationErrors =apiModel.ValidationErrors.Select(ve => ve.ToCoreModel()).ToList()
            };

        private static InputObjectApiModel ToInputObjectApiModel(this ExecutionContext coreModel, ExtensionInputObject inputObject) =>
            new InputObjectApiModel
            {
                Description = inputObject.Description,
                IsProvided = coreModel.ProvidedInputObjects.Contains(inputObject.Name),
                IsRequired = inputObject.IsRequired,
                ObjectTypeName = inputObject.ObjectTypeName,
                ObjectTypeUrl = inputObject.ObjectTypeUrl
            };

        private static OutputObjectApiModel ToOutputObjectApiModel(this ExecutionContext coreModel, ExtensionOutputObject outputObject) =>
            new OutputObjectApiModel
            {
                Description = outputObject.Description,
                IsProvided = coreModel.ProvidedOutputObjects.Contains(outputObject.Name),
                ObjectTypeName = outputObject.ObjectTypeName,
                ObjectTypeUrl = outputObject.ObjectTypeUrl
            };
    }
}
