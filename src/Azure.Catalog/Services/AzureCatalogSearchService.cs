// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Azure.Catalog.Models;
using Draco.Azure.Options;
using Draco.Core.Catalog.Interfaces;
using Draco.Core.Catalog.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draco.Azure.Catalog.Services
{
    public class AzureCatalogSearchService : ICatalogSearchService
    {
        private readonly ISearchIndexClient searchIndexClient;

        public AzureCatalogSearchService(IOptionsSnapshot<AzureSearchOptions<AzureCatalogSearchService>> searchOptionsSnapshot)
        {
            var searchOptions = searchOptionsSnapshot.Value;
            var searchServiceClient = new SearchServiceClient(searchOptions.ServiceName, new SearchCredentials(searchOptions.QueryKey));

            searchIndexClient = searchServiceClient.Indexes.GetClient(searchOptions.IndexName);
        }

        public Task<CatalogSearchResults> SearchAsync(CatalogSearchRequest searchRequest)
        {
            ValidateSearchRequest(searchRequest);

            var searchId = Guid.NewGuid().ToString();
            var filterBuilder = new StringBuilder();

            AppendPublisherNameFilter(filterBuilder, searchRequest);
            AppendCategoryFilter(filterBuilder, searchRequest);
            AppendSubcategoryFilter(filterBuilder, searchRequest);
            AppendTagFilter(filterBuilder, searchRequest);

            var searchParameters = new SearchParameters();

            if (filterBuilder.Length > 0)
            {
                searchParameters.Filter = filterBuilder.ToString();
            }

            searchParameters.IncludeTotalResultCount = true;
            searchParameters.Skip = (searchRequest.PageIndex * searchRequest.PageLength);
            searchParameters.Top = searchRequest.PageLength;

            var searchResults = searchIndexClient.Documents.Search<AzureSearchResult>(
                searchRequest.Query ?? "*", searchParameters);

            return Task.FromResult(ToCatalogSearchResults(searchId, searchRequest, searchResults));
        }

        private CatalogSearchResults ToCatalogSearchResults(string searchId, CatalogSearchRequest searchRequest,
                                                            DocumentSearchResult<AzureSearchResult> searchResults)
        {
            var totalResults = (int)(searchResults.Count.GetValueOrDefault());

            return new CatalogSearchResults
            {
                SearchId = searchId,
                PageIndex = searchRequest.PageIndex, 
                PageLength = searchResults.Results.Count,
                Results = searchResults.Results.Select(r => ToCatalogSearchResult(r.Document)).ToList(),
                TotalPages = (totalResults == 0 ? 0 : ((totalResults + searchRequest.PageLength - 1) / searchRequest.PageLength)),
                TotalResults = totalResults
            };
        }

        private CatalogSearchResult ToCatalogSearchResult(AzureSearchResult azSearchResult) =>
            new CatalogSearchResult
            {
                ActionId = Guid.NewGuid().ToString(), // Future: provided by Personalizer service
                Category = azSearchResult.Category,
                Description = azSearchResult.Description,
                ExtensionId = azSearchResult.ExtensionId,
                ExtensionName = azSearchResult.Name,
                PublisherName = azSearchResult.PublisherName,
                Subcategory = azSearchResult.Subcategory,
                Tags = azSearchResult.Tags
            };

        private void AppendCategoryFilter(StringBuilder filterBuilder, CatalogSearchRequest searchRequest)
        {
            if (string.IsNullOrEmpty(searchRequest.Category) == false)
            {
                if (filterBuilder.Length > 0)
                {
                    filterBuilder.Append(" and ");
                }

                filterBuilder.Append($"category eq '{searchRequest.Category}'");
            }
        }

        private void AppendPublisherNameFilter(StringBuilder filterBuilder, CatalogSearchRequest searchRequest)
        {
            if (string.IsNullOrEmpty(searchRequest.PublisherName) == false)
            {
                if (filterBuilder.Length > 0)
                {
                    filterBuilder.Append(" and ");
                }

                filterBuilder.Append($"publisherName eq '{searchRequest.PublisherName}'");
            }
        }

        private void AppendSubcategoryFilter(StringBuilder filterBuilder, CatalogSearchRequest searchRequest)
        {
            if (string.IsNullOrEmpty(searchRequest.Subcategory) == false)
            {
                if (filterBuilder.Length > 0)
                {
                    filterBuilder.Append(" and ");
                }

                filterBuilder.Append($"subcategory eq '{searchRequest.Subcategory}'");
            }
        }

        private void AppendTagFilter(StringBuilder filterBuilder, CatalogSearchRequest searchRequest)
        {
            if (searchRequest.Tags.Any())
            {
                foreach (var tag in searchRequest.Tags)
                {
                    if (filterBuilder.Length > 0)
                    {
                        filterBuilder.Append(" and ");
                    }

                    filterBuilder.Append($"tags/any(t: t eq '{tag}')");
                }
            }
        }

        private void ValidateSearchRequest(CatalogSearchRequest searchRequest)
        {
            if (searchRequest == null)
            {
                throw new ArgumentNullException(nameof(searchRequest));
            }

            if (searchRequest.PageLength <= 0)
            {
                throw new ArgumentException($"[{nameof(searchRequest.PageLength)}] must be greater than zero (0).",
                                            nameof(searchRequest));
            }

            if (string.IsNullOrEmpty(searchRequest.Query) &&
                string.IsNullOrEmpty(searchRequest.Category))
            {
                throw new ArgumentException($"[{nameof(searchRequest.Query)}] and/or [{nameof(searchRequest.Category)}] must be provided.",
                                            nameof(searchRequest));
            }

            if (string.IsNullOrEmpty(searchRequest.Category) &&
                string.IsNullOrEmpty(searchRequest.Subcategory) == false)
            {
                throw new ArgumentException($"When [{nameof(searchRequest.Subcategory)}] is provided, " +
                                            $"[{nameof(searchRequest.Category)}] must also be provided.",
                                            nameof(searchRequest));
            }
        }
    }
}
