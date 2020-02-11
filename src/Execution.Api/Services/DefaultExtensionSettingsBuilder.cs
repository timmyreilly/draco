using Draco.Execution.Api.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Draco.Execution.Api.Services
{
    public class DefaultExtensionSettingsBuilder : IExtensionSettingsBuilder
    {
        public Task<JObject> BuildExtensionSettingsAsync(IExecutionRequestContext erContext)
        {
            if (erContext == null)
            {
                throw new ArgumentNullException(nameof(erContext));
            }

            return Task.FromResult(erContext.ExecutionProfile.ExtensionSettings);
        }
    }
}
