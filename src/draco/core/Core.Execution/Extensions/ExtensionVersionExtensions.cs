// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using System;

namespace Draco.Core.Execution.Extensions
{
    public static class ExtensionVersionExtensions
    {
        /// <summary>
        /// Creates a unique, canonical execution ID.
        /// </summary>
        /// <param name="extensionVersion">The execution extension version</param>
        /// <param name="tenantId">The executor tenant ID</param>
        /// <returns></returns>
        public static string CreateNewExecutionId(this ExtensionVersion extensionVersion, string tenantId) =>
            $"{extensionVersion.ExtensionId}_{extensionVersion.ExtensionVersionId}_{tenantId}_{Guid.NewGuid()}";
    }
}
