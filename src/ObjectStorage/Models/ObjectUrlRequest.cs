// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models.Interfaces;
using System;

namespace Draco.Core.ObjectStorage.Models
{
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
