// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Catalog.Models;
using System.Threading.Tasks;

namespace Draco.Core.Catalog.Interfaces
{
    /// <summary>
    /// Defines a mechanism for searching the Draco extension catalog.
    /// </summary>
    public interface ICatalogSearchService
    {
        Task<CatalogSearchResults> SearchAsync(CatalogSearchRequest searchRequest);
    }
}
