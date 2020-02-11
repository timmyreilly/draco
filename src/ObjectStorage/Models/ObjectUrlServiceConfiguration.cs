using System;
using System.Collections.Generic;
using System.Text;

namespace Draco.Core.ObjectStorage.Models
{
    public class ObjectUrlServiceConfiguration
    {
        public Dictionary<string, ObjectUrl> InputObjects { get; set; } = new Dictionary<string, ObjectUrl>();
        public Dictionary<string, ObjectUrl> OutputObjects { get; set; } = new Dictionary<string, ObjectUrl>();
    }
}
