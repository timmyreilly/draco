// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.Interfaces;
using Draco.Azure.Models.Cosmos.Extensions;
using Draco.Azure.Options;
using Draco.Core.Models.Interfaces;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Draco.Azure.Models.Cosmos.Repositories
{
    public class CosmosExtensionRepository : BaseCosmosRepository, IExtensionRepository
    {
        // Partition Key == ExtensionId

        public CosmosExtensionRepository(IOptionsSnapshot<CosmosRepositoryOptions<CosmosExtensionRepository>> optionsSnapshot)
            : this(optionsSnapshot.Value) { }

        public CosmosExtensionRepository(ICosmosRepositoryOptions repositoryOptions)
            : base(repositoryOptions) { }

        public Task<Core.Models.Extension> GetExtensionAsync(string extensionId)
        {         
            if (string.IsNullOrEmpty(extensionId))
                throw new ArgumentNullException(nameof(extensionId));

            var extensionDocs = DocumentClient.CreateDocumentQuery<JObject>(
                DocumentCollectionUri,
                " SELECT * " +
                " FROM   c " +
                $"WHERE  c.extensionId = '{extensionId}' ",
                new FeedOptions { PartitionKey = new PartitionKey(extensionId) })
                .ToList();

            var extension = extensionDocs
                .First(o => ((string)o["modelType"]) == ModelTypes.V1.Extension)
                .ToObject<Extension>()
                .ToCoreModel();

            extension.ExtensionVersions = extensionDocs
                .Where(o => ((string)o["modelType"]) == ModelTypes.V1.ExtensionVersion)
                .Select(o => o.ToObject<ExtensionVersion>().ToCoreModel())
                .ToList();

            return Task.FromResult(extension);
        }

        public async Task<Core.Models.ExtensionVersion> GetExtensionVersionAsync(string extensionId, string versionId)
        {
            if (string.IsNullOrEmpty(extensionId))
                throw new ArgumentNullException(nameof(extensionId));

            if (string.IsNullOrEmpty(versionId))
                throw new ArgumentNullException(nameof(versionId));

            var versionDoc = await DocumentClient.ReadDocumentAsync<ExtensionVersion>(
                CreateDocumentUri($"{extensionId}:{versionId}"),
                new RequestOptions { PartitionKey = new PartitionKey(extensionId) });

            return versionDoc.Document?.ToCoreModel();
        }

        public async Task<string> SaveExtensionAsync(Draco.Core.Models.Extension coreExtension)
        {
            if (coreExtension == null)
                throw new ArgumentNullException(
                    nameof(coreExtension));

            // Convert from the 'Core.Models.Extension' to 'Azure.Models.Cosmos.Extension' 
            var extension = coreExtension.ToCosmosModel();

            // Create the extension document
            var extensionDoc = await DocumentClient.UpsertDocumentAsync(DocumentCollectionUri, extension);

            return extension.ExtensionId;
        }

        public async Task<string> SaveExtensionVersionAsync(Draco.Core.Models.ExtensionVersion coreExtensionVersion)
        {
            // Make sure an extension Id is referenced in the version extension document.
            if (string.IsNullOrEmpty(coreExtensionVersion.ExtensionId))
                throw new ArgumentException(
                    "Extension ID is missing in extension version document.",
                    nameof(coreExtensionVersion.ExtensionId));
           
            // Make sure the extension for this extension version exists.
            var extension = await GetExtensionAsync(coreExtensionVersion.ExtensionId);
            if (extension == null)
                throw new ArgumentException(
                    string.Format("Extension ID '{0}' does not exist.", coreExtensionVersion.ExtensionId),
                    nameof(coreExtensionVersion.ExtensionId));
                    
            // Convert from the 'Core.Models.ExtensionVersion' to 'Azure.Models.Cosmos.ExtensionVersion' 
            var extensionVersion = coreExtensionVersion.ToCosmosModel();
        
            // Create the extensionVersion document
            await DocumentClient.UpsertDocumentAsync(DocumentCollectionUri, extensionVersion);

            return extensionVersion.ExtensionVersionId;
        }
    }
}