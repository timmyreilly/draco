// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;

namespace Draco.Execution.Api.Interfaces
{
    public interface IUserContext
    {
        ExecutorContext Executor { get; }
    }
}
