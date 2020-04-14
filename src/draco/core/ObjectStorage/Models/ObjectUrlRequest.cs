// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models.Interfaces;
using System;

namespace Draco.Core.ObjectStorage.Models
{
    /// <summary>
    /// This is the request that execution object URL providers use to generate URLs.
    /// For more information on execution objects, see /doc/architecture/execution-objects.md.
    /// </summary>
    public class ObjectUrlRequest
    {
        public ObjectUrlRequest() { }

        public ObjectUrlRequest(string objectName, IExecutionMetadata execMetadata, 
                                TimeSpan? urlExpirationPeriod = null)
        {
            ObjectName = objectName;
            ExecutionMetadata = execMetadata;
            UrlExpirationPeriod = urlExpirationPeriod;
        }

        public string ObjectName { get; set; }

        public IExecutionMetadata ExecutionMetadata { get; set; }

        public TimeSpan? UrlExpirationPeriod { get; set; }
    }
}
