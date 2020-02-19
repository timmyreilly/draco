// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Draco.Execution.Api.Interfaces
{
    public interface IExecutionRequestContextValidator
    {
        Task ValidateExecutionRequestContextAsync(IExecutionRequestContext erContext);
    }
}
