using Draco.Core.Models;
using Draco.ExtensionManagement.Api.Models;
using System;

namespace Draco.ExtensionManagement.Api.Extensions
{
    public static class ExtensionVersionExtensions
    {
        public static ExtensionVersion ToCoreModel(this ExtensionVersionApiModel apiModel, string extensionId) =>
            new ExtensionVersion
            {
                ExtensionId = extensionId,
                ExtensionVersionId = Guid.NewGuid().ToString(),
                IsActive = true,
                IsLongRunning = apiModel.IsLongRunning,
                ReleaseNotes = apiModel.ReleaseNotes,
                SupportsValidation = apiModel.SupportsValidation,
                Version = apiModel.Version,
                RequestTypeName = apiModel.RequestTypeName,
                ResponseTypeName = apiModel.ResponseTypeName,
                RequestTypeUrl = apiModel.RequestTypeUrl,
                ResponseTypeUrl = apiModel.ResponseTypeUrl
            };

        public static ExtensionVersionApiModel ToApiModel(this ExtensionVersion coreModel, string extensionId) =>
            new ExtensionVersionApiModel
            {
                ExecutionExpirationPeriod = coreModel.ExecutionExpirationPeriod,
                ExtensionId = extensionId,
                Id = coreModel.ExtensionVersionId,
                IsActive = coreModel.IsActive,
                IsLongRunning = coreModel.IsLongRunning,
                ReleaseNotes = coreModel.ReleaseNotes,
                SupportsValidation = coreModel.SupportsValidation,
                Version = coreModel.Version,
                RequestTypeName = coreModel.RequestTypeName,
                ResponseTypeName = coreModel.ResponseTypeName,
                RequestTypeUrl = coreModel.RequestTypeUrl,
                ResponseTypeUrl = coreModel.ResponseTypeUrl
            };
    }
}
