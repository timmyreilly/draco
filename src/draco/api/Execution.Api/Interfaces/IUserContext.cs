// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;

namespace Draco.Execution.Api.Interfaces
{
    /// <summary>
    /// Contains information on the user/executor interacting with the execution API.
    /// </summary>
    public interface IUserContext
    {
        ExecutorContext Executor { get; }
    }
}
