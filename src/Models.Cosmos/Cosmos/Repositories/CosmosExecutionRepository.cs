using Draco.Azure.Interfaces;
using Draco.Azure.Models.Cosmos.Extensions;
using Draco.Azure.Options;
using Draco.Core.Models.Interfaces;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Draco.Azure.Models.Cosmos.Repositories
{
    public class CosmosExecutionRepository : BaseCosmosRepository, IExecutionRepository
    {
        // Partition Key == TenantId

        public CosmosExecutionRepository(IOptionsSnapshot<CosmosRepositoryOptions<CosmosExecutionRepository>> optionsSnapshot)
            : this(optionsSnapshot.Value) { }

        public CosmosExecutionRepository(ICosmosRepositoryOptions repositoryOptions)
            : base(repositoryOptions) { }

        public async Task<Core.Models.Execution> GetExecutionAsync(string executionId, string tenantId)
        {
            if (string.IsNullOrEmpty(executionId))
                throw new ArgumentNullException(nameof(executionId));

            var executionDoc = await DocumentClient.ReadDocumentAsync<Execution>(
                CreateDocumentUri(executionId),
                new RequestOptions { PartitionKey = new PartitionKey(tenantId) });

            return executionDoc.Document?.ToCoreModel();
        }

        public Task<IEnumerable<Core.Models.Execution>> GetExecutionsByTenantAsync(string tenantId)
        {
            if (string.IsNullOrEmpty(tenantId))
                throw new ArgumentNullException(nameof(tenantId));

            var executionDocs = DocumentClient
                .CreateDocumentQuery<Execution>(
                    DocumentCollectionUri,
                    new FeedOptions { PartitionKey = new PartitionKey(tenantId) })
                .Where(e => e.Executor.TenantId == tenantId)
                .ToList();

            return Task.FromResult(executionDocs.Select(e => e.ToCoreModel()));
        }

        public Task<IEnumerable<Core.Models.Execution>> GetExecutionsByUserAsync(string userId, string tenantId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var executionDocs = DocumentClient
                .CreateDocumentQuery<Execution>(
                    DocumentCollectionUri,
                    new FeedOptions { PartitionKey = new PartitionKey(tenantId) })
                .Where(e => e.Executor.UserId == userId)
                .ToList();

            return Task.FromResult(executionDocs.Select(e => e.ToCoreModel()));
        }

        public async Task UpsertExecutionAsync(Core.Models.Execution execution)
        {
            if (execution == null)
                throw new ArgumentNullException(nameof(execution));

            await DocumentClient.UpsertDocumentAsync(
                DocumentCollectionUri,
                execution.ToCosmosModel());
        }
    }
}
