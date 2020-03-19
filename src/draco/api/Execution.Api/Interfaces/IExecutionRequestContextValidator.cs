// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Draco.Execution.Api.Interfaces
{
    /// <summary>
    /// This is a future-looking interface.
    /// The idea here is that there may be additional, execution-model specific validation needed before we push the request into the execution pipeline.
    /// For more information on execution models, see /doc/architecture/execution-models.md.
    /// </summary>
    public interface IExecutionRequestContextValidator
    {
        Task ValidateExecutionRequestContextAsync(IExecutionRequestContext erContext);
    }
}
