// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Draco.Core.Hosting.Interfaces
{
    public interface IServiceModule
    {
        void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    }
}
