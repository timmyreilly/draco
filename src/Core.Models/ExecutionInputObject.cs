using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.Core.Models
{
    public class ExecutionInputObject
    {
        public string ObjectName { get; set; }
        public string ObjectType { get; set; }

        public JObject ObjectMetadata { get; set; }
    }
}
