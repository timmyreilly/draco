﻿namespace Draco.Azure.Interfaces
{
    public interface IAzureSearchOptions
    {
        string IndexName { get; }
        string QueryKey { get; }
        string ServiceName { get; }
    }
}
