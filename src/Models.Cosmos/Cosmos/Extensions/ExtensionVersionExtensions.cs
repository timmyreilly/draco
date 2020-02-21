// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;

namespace Draco.Azure.Models.Cosmos.Extensions
{
    public static class ExtensionVersionExtensions
    {
        public static Core.Models.ExtensionVersion ToCoreModel(this ExtensionVersion cosmosModel) => new Core.Models.ExtensionVersion
        {
            ExecutionExpirationPeriod = cosmosModel.ExecutionExpirationPeriod,
            ExtensionId = cosmosModel.ExtensionId,
            ExtensionVersionId = cosmosModel.ExtensionVersionId,
            InputObjects = cosmosModel.InputObjects,
            IsActive = cosmosModel.IsActive,
            IsLongRunning = cosmosModel.IsLongRunning,
            OutputObjects = cosmosModel.OutputObjects,
            ReleaseNotes = cosmosModel.ReleaseNotes,
            SupportedServices = cosmosModel.SupportedServices,
            SupportsValidation = cosmosModel.SupportsValidation,
            Version = cosmosModel.Version,
            RequestTypeName = cosmosModel.RequestTypeName,
            RequestTypeUrl = cosmosModel.RequestTypeUrl,
            ResponseTypeName = cosmosModel.ResponseTypeName,
            ResponseTypeUrl = cosmosModel.ResponseTypeUrl,
            ExecutionProfiles = cosmosModel.ExecutionProfiles.Select(p => p.Value.ToCoreModel(p.Key)).ToList()
        };

        public static ExtensionVersion ToCosmosModel(this Core.Models.ExtensionVersion coreModel) => new ExtensionVersion
        {
            ExecutionExpirationPeriod = coreModel.ExecutionExpirationPeriod,
            ExtensionId = coreModel.ExtensionId,
            ExtensionVersionId = coreModel.ExtensionVersionId,
            InputObjects = coreModel.InputObjects,
            IsActive = coreModel.IsActive,
            IsLongRunning = coreModel.IsLongRunning,
            OutputObjects = coreModel.OutputObjects,
            ReleaseNotes = coreModel.ReleaseNotes,
            SupportedServices = coreModel.SupportedServices,
            SupportsValidation = coreModel.SupportsValidation,
            Version = coreModel.Version,
            RequestTypeName = coreModel.RequestTypeName,
            RequestTypeUrl = coreModel.RequestTypeUrl,
            ResponseTypeName = coreModel.ResponseTypeName,
            ResponseTypeUrl = coreModel.ResponseTypeUrl,
            ExecutionProfiles = coreModel.ExecutionProfiles.ToDictionary(p => p.ProfileName, p => p.ToCosmosModel())
        };
    }
}
