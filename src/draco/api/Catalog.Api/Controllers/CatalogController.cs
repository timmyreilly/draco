// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Catalog.Api.Models;
using Draco.Core.Catalog.Interfaces;
using Draco.Core.Catalog.Models;
using Draco.Core.Models;
using Draco.Core.Models.Extensions;
using Draco.Core.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Draco.Catalog.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogSearchService catalogSearchService;
        private readonly IExtensionRepository extensionRepository;

        public CatalogController(ICatalogSearchService catalogSearchService, IExtensionRepository extensionRepository)
        {
            this.catalogSearchService = catalogSearchService;
            this.extensionRepository = extensionRepository;
        }

        /// <summary>
        /// Gets available extensions based on the provided full-text search criteria [q]
        /// Added tags as an optional filter
        /// </summary>
        /// <param name="q">The full-text search criteria</param>
        /// <param name="pageIndex">The search results page index (first page is 0)</param>
        /// <param name="pageSize">The search results page size</param>
        /// <response code="200">Search results returned.</response>
        /// <response code="400">See error text for more details.</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(CatalogSearchResultsApiModel), 200)]
        public async Task<IActionResult> SearchAsync([Required] string q, int pageIndex = 0, int pageSize = 10)
        {
            // Check to make sure that paging parameters make sense...

            var errors = ValidatePaging(pageIndex, pageSize).ToList();

            // Check to make sure search criteria was provided...

            if (string.IsNullOrEmpty(q))
            {
                errors.Add($"[q] is required.");
            }

            // If there were any validation errors, respond with [400 Bad Request] + detailed error description...

            if (errors.Any())
            {
                return BadRequest(string.Join(' ', errors));
            }

            var searchRequest = new CatalogSearchRequest
            {
                PageIndex = pageIndex,
                PageLength = pageSize,
                Query = q
            };

            // Run the search...

            var searchResults = await catalogSearchService.SearchAsync(searchRequest);

            // Always respond with [200 OK] even if there aren't any results...

            return Ok(ToDetailApiModel(searchResults));
        }

        /// <summary>
        /// Gets available extensions based on the provided extension [category] and full-text search criteria [q]
        /// </summary>
        /// <param name="category">The extension category name</param>
        /// <param name="q">The full-text search criteria</param>
        /// <param name="pageIndex">The search results page index (first page is 0)</param>
        /// <param name="pageSize">The search results page size</param>
        /// <response code="200">Search results returned.</response>
        /// <response code="400">See error text for more details.</response>
        [HttpGet("search/{category}")]
        [ProducesResponseType(typeof(CatalogSearchResultsApiModel), 200)]
        public async Task<IActionResult> SearchAsync([Required] string category, string q = null, int pageIndex = 0, int pageSize = 10)
        {
            // Check to make sure that the paging parameters make sense...

            var errors = ValidatePaging(pageIndex, pageSize).ToList();

            // If there were any validation errors, respond with [400 Bad Request] + detailed error description...

            if (errors.Any())
            {
                return BadRequest(string.Join(' ', errors));
            }

            var searchRequest = new CatalogSearchRequest
            {
                Category = category,
                PageIndex = pageIndex,
                PageLength = pageSize,
                Query = q
            };

            // Run the search...

            var searchResults = await catalogSearchService.SearchAsync(searchRequest);

            // Always respond with [200 OK] even if there are no results...

            return Ok(ToDetailApiModel(searchResults));
        }

        /// <summary>
        /// Gets available extensions based on the provided extension [category], [subcategory], and full-text search criteria [q]
        /// </summary>
        /// <param name="category">The extension category name</param>
        /// <param name="subcategory">The extension subcategory name</param>
        /// <param name="q">The full-text search criteria</param>
        /// <param name="pageIndex">The search results page index (first page is 0)</param>
        /// <param name="pageSize">The search results page size</param>
        /// <response code="200">Search results returned.</response>
        /// <response code="400">See error text for more details.</response>
        [HttpGet("search/{category}/{subcategory}")]
        [ProducesResponseType(typeof(CatalogSearchResultsApiModel), 200)]
        public async Task<IActionResult> SearchAsync([Required] string category, [Required] string subcategory, 
                                                     string q = null, int pageIndex = 0, int pageSize = 10)
        {
            // Check to make sure that the paging parameters make sense...

            var errors = ValidatePaging(pageIndex, pageSize).ToList();

            // If there were any validation errors, respond with [400 Bad Request] + detailed error description...

            if (errors.Any())
            {
                return BadRequest(string.Join(' ', errors));
            }

            var searchRequest = new CatalogSearchRequest
            {
                Category = category,
                Subcategory = subcategory,
                PageIndex = pageIndex,
                PageLength = pageSize,
                Query = q
            };

            // Run the search...

            var searchResults = await catalogSearchService.SearchAsync(searchRequest);

            // Always return with [200 OK] if there aren't any results...

            return Ok(ToDetailApiModel(searchResults));
        }

        /// <summary>
        /// Gets extension details including available versions and related links
        /// </summary>
        /// <param name="extensionId">The unique extension ID</param>
        /// <param name="searchId">The unique search ID (returned in search results)</param>
        /// <param name="actionId">The unique search result ID (returned in search results)</param>
        /// <response code="200">Extension detail returned.</response>
        /// <response code="404">Extension not found.</response>
        [HttpGet("extensions/{extensionId:guid}")]
        [ProducesResponseType(typeof(ExtensionDetailApiModel), 200)]
        public async Task<IActionResult> GetExtensionAsync([Required] string extensionId, string searchId = null, string actionId = null)
        {
            // Try to get the extension...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we can't find the extension, respond with [404 Not Found]...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            // Otherwise, respond with [200 OK] + extension detail...

            return Ok(ToDetailApiModel(extension));
        }

        /// <summary>
        /// Gets extension version details including supported execution profiles, input objects, output objects, and related links
        /// </summary>
        /// <param name="extensionId">The unique extension ID</param>
        /// <param name="exVersionId">The unique extension version ID</param>
        /// <response code="200">Extension version detail returned.</response>
        /// <response code="404">Extension or extension version not found.</response>
        [HttpGet("extensions/{extensionId:guid}/versions/{exVersionId:guid}")]
        [ProducesResponseType(typeof(ExtensionVersionDetailApiModel), 200)]
        public async Task<IActionResult> GetExtensionVersionAsync([Required] string extensionId, [Required] string exVersionId)
        {
            // Try to get the extension + extension version...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);
            var extensionVersion = extension?.GetExtensionVersion(exVersionId);

            // If we can't find either, respond with [404 Not Found] + specific error description...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            if (extensionVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            // Otherwise, respond with [200 OK] + extension version detail...

            return Ok(ToDetailApiModel(extensionVersion));
        }

        /// <summary>
        /// Gets available extensions based on the provided extension tags search criteria
        /// </summary>
        /// <param name="tags">The tags search criteria</param>
        /// <param name="pageIndex">The search results page index (first page is 0)</param>
        /// <param name="pageSize">The search results page size</param>
        /// <response code="200">Search results returned.</response>
        /// <response code="400">See error text for more details.</response>
        [HttpGet("search/{tags}")]
        [ProducesResponseType(typeof(CatalogSearchResultsApiModel), 200)]
        public async Task<IActionResult> SearchAsync([Required] string tags = null, int pageIndex = 0, int pageSize = 10)
        {
            // Check to make sure that paging parameters make sense...

            var errors = ValidatePaging(pageIndex, pageSize).ToList();

            // Check to make sure search criteria was provided...

            if (string.IsNullOrEmpty(tags))
            {
                errors.Add($"[tags] are required.");
            }

            // If there were any validation errors, respond with [400 Bad Request] + detailed error description...

            if (errors.Any())
            {
                return BadRequest(string.Join(' ', errors));
            }

            var searchRequest = new CatalogSearchRequest
            {
                PageIndex = pageIndex,
                PageLength = pageSize,
                Tags = tags 
            };

            // Run the search...

            var searchResults = await catalogSearchService.SearchAsync(searchRequest);

            // Always respond with [200 OK] even if there aren't any results...

            return Ok(ToDetailApiModel(searchResults));
        }

        private string GetGetExtensionDetailUrl(string extensionId, string searchId, string actionId) =>
            (Request == null) ? (default) :
            $"{Request.Scheme}://{Request.Host}/catalog/extensions/{extensionId}?searchId={WebUtility.UrlEncode(searchId)}&actionId={WebUtility.UrlEncode(actionId)}";

        private string GetGetExtensionDetailUrl(string extensionId) =>
            (Request == null) ? (default) :
            $"{Request.Scheme}://{Request.Host}/catalog/extensions/{extensionId}";

        private string GetGetExtensionVersionDetailUrl(string extensionId, string exVersionId) =>
            (Request == null) ? (default) :
            $"{Request.Scheme}://{Request.Host}/catalog/extensions/{extensionId}/versions/{exVersionId}";

        private ExtensionVersionListItemModel ToListItemApiModel(ExtensionVersion exVersion) =>
            new ExtensionVersionListItemModel
            {
                DetailUrl = GetGetExtensionVersionDetailUrl(exVersion.ExtensionId, exVersion.ExtensionVersionId),
                Id = exVersion.ExtensionVersionId,
                ReleaseNotes = exVersion.ReleaseNotes,
                Version = exVersion.Version?.ToString()
            };

        private CatalogSearchResultsApiModel ToDetailApiModel(CatalogSearchResults searchResults) =>
            new CatalogSearchResultsApiModel
            {
                PageIndex = searchResults.PageIndex,
                PageSize = searchResults.PageLength,
                TotalPages = searchResults.TotalPages,
                TotalResults = searchResults.TotalResults,
                Results = searchResults.Results.Select(r => ToDetailApiModel(r, searchResults.SearchId)).ToList()
            };

        private CatalogSearchResultApiModel ToDetailApiModel(CatalogSearchResult searchResult, string searchId) =>
            new CatalogSearchResultApiModel
            {
                Category = searchResult.Category,
                Description = searchResult.Description,
                Id = searchResult.ExtensionId,
                Name = searchResult.ExtensionName,
                PublisherName = searchResult.PublisherName,
                Subcategory = searchResult.Subcategory,
                Tags = searchResult.Tags,
                ExtensionDetailUrl = GetGetExtensionDetailUrl(searchResult.ExtensionId, searchId, searchResult.ActionId)
            };

        private ExecutionProfileApiModel ToDetailApiModel(ExecutionProfile execProfile) =>
            new ExecutionProfileApiModel
            {
                ClientConfiguration = execProfile.ClientConfiguration,
                Description = execProfile.ProfileDescription,
                ExecutionMode = execProfile.ExecutionMode.ToString(),
                SupportedPriorities = execProfile.SupportedPriorities.ToString().Split(',').Select(sp => sp.Trim()).ToList()
            };

        private ExtensionVersionDetailApiModel ToDetailApiModel(ExtensionVersion exVersion) =>
            new ExtensionVersionDetailApiModel
            {
                ExtensionDetailUrl = GetGetExtensionDetailUrl(exVersion.ExtensionId),
                IsLongRunning = exVersion.IsLongRunning,
                ReleaseNotes = exVersion.ReleaseNotes,
                SupportsValidation = exVersion.SupportsValidation,
                Version = exVersion.Version?.ToString(),
                VersionId = exVersion.ExtensionVersionId,
                RequestTypeName = exVersion.RequestTypeName,
                RequestTypeUrl = exVersion.RequestTypeUrl,
                ResponseTypeName = exVersion.ResponseTypeName,
                ResponseTypeUrl = exVersion.ResponseTypeUrl,
                ExecutionProfiles = exVersion.ExecutionProfiles.ToDictionary(
                    ep => ep.ProfileName,
                    ep => ToDetailApiModel(ep)),
                InputObjects = exVersion.InputObjects.ToDictionary(
                    io => io.Name,
                    io => ToApiModel(io)),
                OutputObjects = exVersion.OutputObjects.ToDictionary(
                    oo => oo.Name,
                    oo => ToApiModel(oo))
            };

        private ExtensionDetailApiModel ToDetailApiModel(Extension extension)
        {
            var detailModel =
                new ExtensionDetailApiModel
                {
                    AdditionalInformationUrls = extension.AdditionalInformationUrls,
                    Category = extension.Category,
                    CopyrightNotice = extension.CopyrightNotice,
                    ExtensionCoverImageUrl = extension.ExtensionCoverImageUrl,
                    ExtensionId = extension.ExtensionId,
                    ExtensionLogoUrl = extension.ExtensionLogoUrl,
                    Name = extension.Name,
                    PublisherName = extension.PublisherName,
                    Subcategory = extension.Subcategory,
                    Tags = extension.Tags,
                    ExtensionVersions = extension.ExtensionVersions.OrderByDescending(ev => ev.Version).Select(ev => ToListItemApiModel(ev)).ToList()
                };

            if (detailModel.ExtensionVersions.Any())
            {
                detailModel.ExtensionVersions.First().IsCurrent = true;
            }

            return detailModel;
        }

        private InputObjectApiModel ToApiModel(ExtensionInputObject inputObject) =>
            new InputObjectApiModel
            {
                Description = inputObject.Description,
                IsRequired = inputObject.IsRequired,
                ObjectTypeName = inputObject.ObjectTypeName,
                ObjectTypeUrl = inputObject.ObjectTypeUrl
            };

        private OutputObjectApiModel ToApiModel(ExtensionOutputObject outputObject) =>
            new OutputObjectApiModel
            {
                Description = outputObject.Description,
                ObjectTypeName = outputObject.ObjectTypeName,
                ObjectTypeUrl = outputObject.ObjectTypeUrl
            };

        private IEnumerable<string> ValidatePaging(int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
            {
                yield return "[pageIndex] must be greater than or equal to zero (0).";
            }

            if (pageSize <= 0)
            {
                yield return "[pageSize] must be greater than zero (0).";
            }
        }
    }
}
