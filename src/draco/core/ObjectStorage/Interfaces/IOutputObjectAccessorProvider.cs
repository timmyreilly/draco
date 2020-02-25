// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.ObjectStorage.Models;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Draco.Core.ObjectStorage.Interfaces
{
    public interface IOutputObjectAccessorProvider
    {
        Task<JObject> GetReadableAccessorAsync(OutputObjectAccessorRequest accessorRequest);
        Task<JObject> GetWritableAccessorAsync(OutputObjectAccessorRequest accessorRequest);
    }
}
