// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Draco.Core.Models
{
    public class Extension
    {
        public string ExtensionId { get; set; }
        public string Name { get; set; }
        public string PublisherName { get; set; }
        public string CopyrightNotice { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string ExtensionLogoUrl { get; set; }
        public string ExtensionCoverImageUrl { get; set; }

        public bool IsActive { get; set; }

        public Dictionary<string, string> AdditionalInformationUrls { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Features { get; set; } = new Dictionary<string, string>();

        public List<ExtensionVersion> ExtensionVersions { get; set; } = new List<ExtensionVersion>();

        public List<string> Tags { get; set; } = new List<string>();

        public override string ToString() => ExtensionId;
    }
}
