using Draco.Core.ObjectStorage.Enumerations;
using System;

namespace Draco.Azure.ObjectStorage.Models
{
    public class AzureObjectUrlRequest
    {
        public string TenantId { get; set; }
        public string ContainerName { get; set; }
        public string BlobName { get; set; }

        public TimeSpan ExpirationPeriod { get; set; }

        public ObjectAccessMode AccessMode { get; set; }
    }
}
