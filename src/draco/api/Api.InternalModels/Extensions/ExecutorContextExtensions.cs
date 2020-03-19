// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using System.Collections.Generic;

namespace Draco.Api.InternalModels.Extensions
{
    /// <summary>
    /// Extension methods for validating executor context API models and
    /// converting to/from executor context API/core models
    /// </summary>
    public static class ExecutorContextExtensions
    {
        /// <summary>
        /// Converts an executor context core model to an API model
        /// </summary>
        /// <param name="coreModel"></param>
        /// <returns></returns>
        public static ExecutorContextApiModel ToApiModel(this ExecutorContext coreModel) =>
            new ExecutorContextApiModel
            {
                TenantId = coreModel.TenantId,
                UserId = coreModel.UserId
            };

        /// <summary>
        /// Converts an executor context API model to a core model
        /// </summary>
        /// <param name="apiModel"></param>
        /// <returns></returns>
        public static ExecutorContext ToCoreModel(this ExecutorContextApiModel apiModel) =>
            new ExecutorContext
            {
                TenantId = apiModel.TenantId,
                UserId = apiModel.UserId
            };

        /// <summary>
        /// Validates an executor context API model
        /// </summary>
        /// <param name="apiModel"></param>
        /// <returns></returns>
        public static IEnumerable<string> ValidateApiModel(this ExecutorContextApiModel apiModel)
        {
            if (string.IsNullOrEmpty(apiModel.TenantId))
            {
                yield return "[tenantId] is required.";
            }

            if (string.IsNullOrEmpty(apiModel.UserId))
            {
                yield return "[userId] is required.";
            }
        }
    }
}
