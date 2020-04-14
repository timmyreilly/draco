// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.Core.Hosting.Interfaces
{
    /// <summary>
    /// Defines a mechanism for registering a logical grouping of service(s) with the
    /// hosting application's service collection.
    /// </summary>
    public interface IServiceModule
    {
        void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    }
}
