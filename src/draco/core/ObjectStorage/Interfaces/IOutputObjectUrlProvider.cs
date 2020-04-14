// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Core.ObjectStorage.Interfaces
{
    /// <summary>
    /// Defines a generic mechanism for creating URLs that can be used to access output objects.
    /// These URLs are later used to create output object accesssors.
    /// For more information on output objects, see /doc/architecture/execution-objects.md.
    /// </summary>
    public interface IOutputObjectUrlProvider : IObjectUrlProvider { }
}
