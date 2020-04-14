// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.Core.Models.Interfaces;
using System;

namespace Draco.Core.ObjectStorage.Models
{
    /// <summary>
    /// This is the request that output object accessor providers use to generate accessors.
    /// For more information on object accessors, see /doc/architecture/execution-objects.md#accessors.
    /// </summary>
    public class OutputObjectAccessorRequest
    {
        public ExtensionOutputObject ObjectMetadata { get; set; }

        public IExecutionMetadata ExecutionMetadata { get; set; }

        public string ObjectProviderName { get; set; }
        public string SignatureRsaKeyXml { get; set; }

        public TimeSpan? ExpirationPeriod { get; set; }
    }
}
