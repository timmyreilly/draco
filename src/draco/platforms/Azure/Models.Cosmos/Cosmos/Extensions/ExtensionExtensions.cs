// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Azure.Models.Cosmos.Extensions
{
    public static class ExtensionExtensions
    {
        public static Core.Models.Extension ToCoreModel(this Extension cosmosModel) => new Core.Models.Extension
        {
            AdditionalInformationUrls = cosmosModel.AdditionalInformationUrls,
            CopyrightNotice = cosmosModel.CopyrightNotice,
            Description = cosmosModel.Description,
            ExtensionCoverImageUrl = cosmosModel.ExtensionCoverImageUrl,
            ExtensionId = cosmosModel.ExtensionId,
            ExtensionLogoUrl = cosmosModel.ExtensionLogoUrl,
            IsActive = cosmosModel.IsActive,
            Name = cosmosModel.Name,
            PublisherName = cosmosModel.PublisherName,
            Tags = cosmosModel.Tags,
            Features = cosmosModel.Features,
            Category = cosmosModel.Category,
            Subcategory = cosmosModel.Subcategory
        };

        public static Extension ToCosmosModel(this Core.Models.Extension coreModel) => new Extension
        {
            AdditionalInformationUrls = coreModel.AdditionalInformationUrls,
            CopyrightNotice = coreModel.CopyrightNotice,
            Description = coreModel.Description,
            ExtensionCoverImageUrl = coreModel.ExtensionCoverImageUrl,
            ExtensionId = coreModel.ExtensionId,
            ExtensionLogoUrl = coreModel.ExtensionLogoUrl,
            IsActive = coreModel.IsActive,
            Name = coreModel.Name,
            PublisherName = coreModel.PublisherName,
            Tags = coreModel.Tags,
            Features = coreModel.Features,
            Category = coreModel.Category,
            Subcategory = coreModel.Subcategory
        };
    }
}
