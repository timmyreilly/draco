// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Draco.Core.Services.Interfaces
{
    public interface IExecutionServiceProviderRegistry : IDictionary<string, Func<IServiceProvider, IExecutionServiceProvider>>
    {
    }
}
