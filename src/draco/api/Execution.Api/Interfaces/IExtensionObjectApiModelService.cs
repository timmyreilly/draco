// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.Execution.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Draco.Execution.Api.Interfaces
{
    /// <summary>
    /// For extensions that expect input objects and/or generate output objects, this interface generates metadata
    /// including object definitions and accessors (see /doc/architecture/execution-objects.md#object-accessors)
    /// that are provided to the client to upload input objects and/or download output objects.
    /// </summary>
    public interface IExtensionObjectApiModelService
    {
        /// <summary>
        /// Generets an input object metadata dictionary including definitions and accessors that clients
        /// use to uplaod the appropriate input object(s).
        /// </summary>
        /// <param name="inputObjects"></param>
        /// <param name="execution"></param>
        /// <returns></returns>
        Task<Dictionary<string, InputObjectApiModel>> CreateInputObjectDictionaryAsync(
            IEnumerable<ExtensionInputObject> inputObjects,
            Core.Models.Execution execution);

        /// <summary>
        /// Generates an output object metadata dictionary including definitions and accessors that clietns
        /// use to download provided output object(s).
        /// </summary>
        /// <param name="outputObjects"></param>
        /// <param name="execution"></param>
        /// <returns></returns>
        Task<Dictionary<string, OutputObjectApiModel>> CreateOutputObjectDictionaryAsync(
            IEnumerable<ExtensionOutputObject> outputObjects,
            Core.Models.Execution execution);
    }
}
