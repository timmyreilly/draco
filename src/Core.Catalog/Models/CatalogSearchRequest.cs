// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Draco.Core.Catalog.Models
{
    public class CatalogSearchRequest
    {
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string PublisherName { get; set; }
        public string Query { get; set; }

        public int PageIndex { get; set; }
        public int PageLength { get; set; }

        public Dictionary<string, string> ClientCapabilities { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> ClientFeatures { get; set; } = new Dictionary<string, string>();

        public List<string> Tags { get; set; } = new List<string>();
    }
}
