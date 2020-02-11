using Draco.Core.Models;
using Draco.Execution.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Draco.Execution.Api.Interfaces
{
    public interface IExtensionObjectApiModelService
    {
        Task<Dictionary<string, InputObjectApiModel>> CreateInputObjectDictionaryAsync(
            IEnumerable<ExtensionInputObject> inputObjects,
            Core.Models.Execution execution);

        Task<Dictionary<string, OutputObjectApiModel>> CreateOutputObjectDictionaryAsync(
            IEnumerable<ExtensionOutputObject> outputObjects,
            Core.Models.Execution execution);
    }
}
