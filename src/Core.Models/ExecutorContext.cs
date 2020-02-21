// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Draco.Core.Models
{
    public class ExecutorContext
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("tenantId")]
        public string TenantId { get; set; }
    }
}
