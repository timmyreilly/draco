// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json.Linq;

namespace Draco.Core.Models
{
    public class ExecutionInputObject
    {
        public string ObjectName { get; set; }
        public string ObjectType { get; set; }

        public JObject ObjectMetadata { get; set; }
    }
}
