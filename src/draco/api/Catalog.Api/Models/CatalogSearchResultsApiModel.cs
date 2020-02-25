// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Draco.Catalog.Api.Models
{
    public class CatalogSearchResultsApiModel
    {
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("totalResults")]
        public int TotalResults { get; set; }

        [JsonProperty("results")]
        public List<CatalogSearchResultApiModel> Results { get; set; }
    }
}
