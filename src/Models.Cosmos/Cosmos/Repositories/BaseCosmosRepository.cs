using Draco.Azure.Interfaces;
using Microsoft.Azure.Documents.Client;
using System;

namespace Draco.Azure.Models.Cosmos.Repositories
{
    public abstract class BaseCosmosRepository
    {
        protected BaseCosmosRepository(ICosmosRepositoryOptions repositoryOptions)
        {
            RepositoryOptions = repositoryOptions;
            DocumentClient = new DocumentClient(new Uri(RepositoryOptions.EndpointUri), RepositoryOptions.AccessKey);
            DocumentCollectionUri = UriFactory.CreateDocumentCollectionUri(RepositoryOptions.DatabaseName, RepositoryOptions.CollectionName);
        }

        protected Uri DocumentCollectionUri { get; }

        protected DocumentClient DocumentClient { get; }

        protected ICosmosRepositoryOptions RepositoryOptions { get; }

        protected Uri CreateDocumentUri(string documentId)
        {
            if (string.IsNullOrEmpty(documentId))
            {
                throw new ArgumentNullException(nameof(documentId));
            }

            return UriFactory.CreateDocumentUri(RepositoryOptions.DatabaseName, RepositoryOptions.CollectionName, documentId);
        }
    }
}
