// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;

namespace Draco.Execution.Api.Interfaces
{
    /// <summary>
    /// Contains information on the user/executor interacting with the execution API.
    /// Essentially, this interface is an abstraction between the execution API and your identity provider.
    /// As Draco itself doesn't mandate a specific identity provider, you'll need to wire up this dependency based on the identity provider that you choose to implement.
    /// In the future, we plan on offering identity provider-specific guidance for doing so.
    /// For more information on Draco's approach to identity, see /doc/README.md#identity.
    /// </summary>
    public interface IUserContext
    {
        ExecutorContext Executor { get; }
    }
}
