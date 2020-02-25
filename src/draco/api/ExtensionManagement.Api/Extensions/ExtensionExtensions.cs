// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.ExtensionManagement.Api.Models;
using System;

namespace Draco.ExtensionManagement.Api.Extensions
{
    public static class ExtensionExtensions
    {
        public static Extension ToCoreModel(this ExtensionApiModel apiModel) =>
            new Extension
            {
                AdditionalInformationUrls = apiModel.AdditionalInformationUrls,
                CopyrightNotice = apiModel.CopyrightNotice,
                Description = apiModel.Description,
                ExtensionCoverImageUrl = apiModel.ExtensionCoverImageUrl,
                ExtensionId = Guid.NewGuid().ToString(),
                ExtensionLogoUrl = apiModel.ExtensionLogoUrl,
                IsActive = true,
                Name = apiModel.Name,
                PublisherName = apiModel.PublisherName,
                Tags = apiModel.Tags,
                Features = apiModel.Features,
                Category = apiModel.Category,
                Subcategory = apiModel.Subcategory
            };

        public static ExtensionApiModel ToApiModel(this Extension coreModel) =>
            new ExtensionApiModel
            {
                AdditionalInformationUrls = coreModel.AdditionalInformationUrls,
                CopyrightNotice = coreModel.CopyrightNotice,
                Description = coreModel.Description,
                ExtensionCoverImageUrl = coreModel.ExtensionCoverImageUrl,
                ExtensionLogoUrl = coreModel.ExtensionLogoUrl,
                Id = coreModel.ExtensionId,
                IsActive = coreModel.IsActive,
                PublisherName = coreModel.PublisherName,
                Name = coreModel.Name,
                Tags = coreModel.Tags,
                Features = coreModel.Features,
                Category = coreModel.Category,
                Subcategory = coreModel.Subcategory
            };
    }
}
