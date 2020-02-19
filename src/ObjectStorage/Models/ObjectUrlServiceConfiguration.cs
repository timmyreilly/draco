// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Draco.Core.ObjectStorage.Models
{
    public class ObjectUrlServiceConfiguration
    {
        public Dictionary<string, ObjectUrl> InputObjects { get; set; } = new Dictionary<string, ObjectUrl>();
        public Dictionary<string, ObjectUrl> OutputObjects { get; set; } = new Dictionary<string, ObjectUrl>();
    }
}
