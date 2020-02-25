// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Draco.Api.InternalModels
{
    public class ValidationErrorApiModel
    {
        [JsonProperty("errorId")]
        public string ErrorId { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("errorData")]
        public JObject ErrorData { get; set; }
    }
}
