// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Draco.Api.InternalModels
{
    /// <summary>
    /// Internal API model for core executor context -- src/draco/core/Core.Models/ExecutorContext.cs
    /// </summary>
    public class ExecutorContextApiModel
    {
        [JsonProperty("tenantId")]
        public string TenantId { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}
