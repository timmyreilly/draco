// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.ObjectStorage.Models;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Draco.Core.ObjectStorage.Interfaces
{
    /// <summary>
    /// Defines a generic, execution-scoped mechanism for creating input object accessors.
    /// For more information on object accessors, see /doc/architecture/execution-objects.md#accessors.
    /// </summary>
    public interface IInputObjectAccessorProvider
    {
        Task<JObject> GetReadableAccessorAsync(InputObjectAccessorRequest accessorRequest);
        Task<JObject> GetWritableAccessorAsync(InputObjectAccessorRequest accessorRequest);
    }
}
