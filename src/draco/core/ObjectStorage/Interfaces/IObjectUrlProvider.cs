// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.ObjectStorage.Models;
using System.Threading.Tasks;

namespace Draco.Core.ObjectStorage.Interfaces
{
    /// <summary>
    /// Defines a generic mechanism for creating URLs that can be used to access execution objects.
    /// These URLs are later used to create execution object accesssors.
    /// For more information on execution objects, see /doc/architecture/execution-objects.md.
    /// </summary>
    public interface IObjectUrlProvider
    {
        Task<ObjectUrl> GetReadableUrlAsync(ObjectUrlRequest urlRequest);
        Task<ObjectUrl> GetWritableUrlAsync(ObjectUrlRequest urlRequest);
    }
}
