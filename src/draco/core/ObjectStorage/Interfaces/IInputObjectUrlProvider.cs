// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Draco.Core.ObjectStorage.Interfaces
{
    /// <summary>
    /// Defines a generic mechanism for creating URLs that can be used to access input objects.
    /// These URLs are later used to create input object accesssors.
    /// For more information on input objects, see /doc/architecture/execution-objects.md.
    /// </summary>
    public interface IInputObjectUrlProvider : IObjectUrlProvider { }
}
