// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Draco.Execution.Api.Interfaces
{
    public interface IExtensionSettingsBuilder
    {
        Task<JObject> BuildExtensionSettingsAsync(IExecutionRequestContext erContext);
    }
}
