// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using System.Collections.Generic;

namespace Draco.Api.InternalModels.Extensions
{
    public static class ExecutorContextExtensions
    {
        public static ExecutorContextApiModel ToApiModel(this ExecutorContext coreModel) =>
            new ExecutorContextApiModel
            {
                TenantId = coreModel.TenantId,
                UserId = coreModel.UserId
            };

        public static ExecutorContext ToCoreModel(this ExecutorContextApiModel apiModel) =>
            new ExecutorContext
            {
                TenantId = apiModel.TenantId,
                UserId = apiModel.UserId
            };

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
