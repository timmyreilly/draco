// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Draco.Execution.Api.Interfaces
{
    /// <summary>
    /// Builds execution model-specific extension settings used later by the appropriate execution adapter and extension.
    /// </summary>
    public interface IExtensionSettingsBuilder
    {
        Task<JObject> BuildExtensionSettingsAsync(IExecutionRequestContext erContext);
    }
}
