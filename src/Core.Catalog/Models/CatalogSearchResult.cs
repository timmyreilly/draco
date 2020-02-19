// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Draco.Core.Catalog.Models
{
    public class CatalogSearchResult
    {
        public string ActionId { get; set; }
        public string ExtensionId { get; set; }
        public string ExtensionName { get; set; }
        public string PublisherName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }

        public List<string> Tags = new List<string>();
    }
}
