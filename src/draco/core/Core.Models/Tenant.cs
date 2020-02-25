// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Core.Models
{
    public class Tenant
    {
        public string TenantId { get; set; }
        public string TenantName { get; set; }

        public bool IsActive { get; set; }
    }
}
