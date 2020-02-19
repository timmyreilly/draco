// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Draco.Core.Catalog.Models
{
    public class CatalogSearchResults
    {
        public string SearchId { get; set; }

        public int PageIndex { get; set; }
        public int PageLength { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }

        public List<CatalogSearchResult> Results { get; set; } = new List<CatalogSearchResult>();
    }
}
