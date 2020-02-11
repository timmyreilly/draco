﻿using Draco.Core.Catalog.Models;
using System.Threading.Tasks;

namespace Draco.Core.Catalog.Interfaces
{
    public interface ICatalogSearchService
    {
        Task<CatalogSearchResults> SearchAsync(CatalogSearchRequest searchRequest);
    }
}
