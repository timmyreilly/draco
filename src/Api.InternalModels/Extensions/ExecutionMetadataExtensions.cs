// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.Core.Models.Interfaces;
using System.Collections.Generic;

namespace Draco.Api.InternalModels.Extensions
{
    public static class ExecutionMetadataExtensions
    {
        public static ExecutionMetadataApiModel ToApiModel(this IExecutionMetadata coreModel) =>
            new ExecutionMetadataApiModel
            {
                ExecutionId = coreModel.ExecutionId,
                ExecutionProfileName = coreModel.ExecutionProfileName,
                Executor = coreModel.Executor.ToApiModel(),
                ExtensionId = coreModel.ExtensionId,
                ExtensionVersionId = coreModel.ExtensionVersionId,
                Priority = coreModel.Priority
            };

        public static IExecutionMetadata ToCoreModel(this ExecutionMetadataApiModel apiModel) =>
            new ExecutionMetadata
            {
                ExecutionId = apiModel.ExecutionId,
                ExecutionProfileName = apiModel.ExecutionProfileName,
                Executor = apiModel.Executor.ToCoreModel(),
                ExtensionId = apiModel.ExtensionId,
                ExtensionVersionId = apiModel.ExtensionVersionId,
                Priority = apiModel.Priority
            };

        public static IEnumerable<string> ValidateApiModel(this ExecutionMetadataApiModel apiModel)
        {
            if (string.IsNullOrEmpty(apiModel.ExecutionId))
            {
                yield return "[executionId] is required.";
            }

            if (string.IsNullOrEmpty(apiModel.ExecutionProfileName))
            {
                yield return "[executionProfileName] is required.";
            }

            if (string.IsNullOrEmpty(apiModel.ExtensionId))
            {
                yield return "[extensionId] is required.";
            }

            if (string.IsNullOrEmpty(apiModel.ExtensionVersionId))
            {
                yield return "[extensionVersionId] is required.";
            }

            if (apiModel.Executor == null)
            {
                yield return "[executor] is required.";
            }
            else
            {
                foreach (var exError in apiModel.Executor.ValidateApiModel())
                {
                    yield return $"[executor]: {exError}";
                }
            }
        }
    }
}
