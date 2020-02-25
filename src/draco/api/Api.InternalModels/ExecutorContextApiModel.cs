// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Draco.Api.InternalModels
{
    public class ExecutorContextApiModel
    {
        [JsonProperty("tenantId")]
        public string TenantId { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}
