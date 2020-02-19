// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json.Linq;

namespace Draco.Core.Execution.Models
{
    public class HttpExecutionValidationError
    {
        public string ErrorId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public JObject ErrorData { get; set; }
    }
}