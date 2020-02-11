using Draco.Core.Models;
using Draco.Core.Models.Interfaces;
using System;

namespace Draco.Core.ObjectStorage.Models
{
    public class OutputObjectAccessorRequest
    {
        public ExtensionOutputObject ObjectMetadata { get; set; }

        public IExecutionMetadata ExecutionMetadata { get; set; }

        public string ObjectProviderName { get; set; }
        public string SignatureRsaKeyXml { get; set; }

        public TimeSpan? ExpirationPeriod { get; set; }
    }
}
