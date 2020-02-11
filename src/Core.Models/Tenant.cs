using System.Collections.Generic;

namespace Draco.Core.Models
{
    public class Tenant
    {
        public string TenantId { get; set; }
        public string TenantName { get; set; }

        public bool IsActive { get; set; }
    }
}
