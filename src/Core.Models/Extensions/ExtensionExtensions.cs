// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;

namespace Draco.Core.Models.Extensions
{
    public static class ExtensionExtensions
    {
        public static ExtensionVersion GetExtensionVersion(this Extension extension, string extensionVersionId) =>
            extension.ExtensionVersions.SingleOrDefault(ev => (ev.ExtensionVersionId == extensionVersionId));
    }
}
