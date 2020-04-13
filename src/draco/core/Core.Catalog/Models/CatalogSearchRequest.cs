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

        // This is the full-text query string.
        public string Query { get; set; }

        public int PageIndex { get; set; }
        public int PageLength { get; set; }

        // TODO: Implement search support for taking relevant client capabilities (client application version, hardware, additional context)
        //       into account when searching for extensions.

        public Dictionary<string, string> ClientCapabilities { get; set; } = new Dictionary<string, string>();

        // TODO: Implement reinforcement learning (initially Azure Cognitive Services Personalizer) to intelligently customize 
        //       extension search results. [ClientFeatures] is an arbitrary key/value collection passed into the search service
        //       to drive intelligent recommendations.

        public Dictionary<string, string> ClientFeatures { get; set; } = new Dictionary<string, string>();

        // TODO: Implement tag-specific search (GH issue #148).

        public List<string> Tags { get; set; } = new List<string>();
    }
}
