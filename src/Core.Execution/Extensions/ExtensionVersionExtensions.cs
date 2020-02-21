// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using System;

namespace Draco.Core.Execution.Extensions
{
    public static class ExtensionVersionExtensions
    {
        public static string CreateNewExecutionId(this ExtensionVersion extensionVersion, string tenantId) =>
            $"{extensionVersion.ExtensionId}_{extensionVersion.ExtensionVersionId}_{tenantId}_{Guid.NewGuid()}";
    }
}
