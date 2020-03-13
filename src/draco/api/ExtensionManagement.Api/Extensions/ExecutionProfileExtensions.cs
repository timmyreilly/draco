// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.Core.Models.Enumerations;
using Draco.ExtensionManagement.Api.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draco.ExtensionManagement.Api.Extensions
{
    public static class ExecutionProfileExtensions
    {
        public static ExecutionProfile ToCoreModel(this ExecutionProfileApiModel apiModel) =>
            new ExecutionProfile
            {
                ClientConfiguration = JObject.FromObject(apiModel.ClientConfiguration),
                ExecutionMode = Enum.Parse<ExecutionMode>(apiModel.ExecutionMode, true),
                ExecutionModelName = apiModel.ExecutionModelName,
                ExtensionSettings = JObject.FromObject(apiModel.ExtensionSettings),
                ObjectProviderName = apiModel.ObjectProviderName,
                ProfileDescription = apiModel.Description,
                ProfileName = apiModel.Name,
                IsActive = apiModel.IsActive,
                SupportedPriorities = Enum.Parse<ExecutionPriority>(string.Join(", ", apiModel.SupportedPriorities), true)
            };

        public static ExecutionProfileApiModel ToApiModel(this ExecutionProfile coreModel, string extensionId, string exVersionId) =>
            new ExecutionProfileApiModel
            {
                ClientConfiguration = coreModel.ClientConfiguration?.ToObject<Dictionary<string, string>>(),
                Description = coreModel.ProfileDescription,
                ExecutionMode = coreModel.ExecutionMode.ToString(),
                ExecutionModelName = coreModel.ExecutionModelName,
                ExtensionId = extensionId,
                ExtensionSettings = coreModel.ExtensionSettings?.ToObject<Dictionary<string, string>>(),
                ExtensionVersionId = exVersionId,
                Name = coreModel.ProfileName,
                ObjectProviderName = coreModel.ObjectProviderName,
                IsActive = coreModel.IsActive,
                SupportedPriorities = coreModel.SupportedPriorities.ToString().Split(',').Select(p => p.Trim()).ToList()
            };
    }
}
