// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Draco.Core.Execution.Adapters
{
    /// <summary>
    /// Base class for any execution adapters that support input/output execution objects.
    /// </summary>
    public abstract class BaseExecutionAdapter
    {
        private readonly IInputObjectAccessorProvider inputObjectAccessorProvider;
        private readonly IOutputObjectAccessorProvider outputObjectAccessorProvider;

        protected BaseExecutionAdapter(IInputObjectAccessorProvider inputObjectAccessorProvider,
                                       IOutputObjectAccessorProvider outputObjectAccessorProvider)
        {
            this.inputObjectAccessorProvider = inputObjectAccessorProvider;
            this.outputObjectAccessorProvider = outputObjectAccessorProvider;
        }

        /// <summary>
        /// Creates an execution-scoped collection of input object accessors to be provided to the target extension.
        /// For more information on execution objects, see /doc/architecture/execution-objects.md.
        /// </summary>
        /// <param name="execRequest">The execution request</param>
        /// <returns></returns>
        protected virtual async Task<Dictionary<string, InputObjectAccessor>> CreateInputObjectAccessorDictionaryAsync(ExecutionRequest execRequest)
        {
            var accessorDictionary = new Dictionary<string, InputObjectAccessor>();

            foreach (var providedInputObjectName in execRequest.ProvidedInputObjects)
            {
                var inputObject = execRequest.InputObjects[providedInputObjectName];

                var accessorRequest = new InputObjectAccessorRequest
                {
                    ExecutionMetadata = execRequest,
                    ExpirationPeriod = execRequest.ExecutionTimeoutDuration,
                    ObjectMetadata = inputObject,
                    ObjectProviderName = execRequest.ObjectProviderName,
                    SignatureRsaKeyXml = execRequest.SignatureRsaKeyXml
                };

                var accessor = await inputObjectAccessorProvider.GetReadableAccessorAsync(accessorRequest);

                accessorDictionary[providedInputObjectName] = new InputObjectAccessor(inputObject, accessor);
            }

            return accessorDictionary;
        }

        /// <summary>
        /// Creates an execution-scoped collection of output object accessors to be provided to the target extension.
        /// For more information on execution objects, see /doc/architecture/execution-objects.md.
        /// </summary>
        /// <param name="execRequest">The execution request</param>
        /// <returns></returns>
        protected virtual async Task<Dictionary<string, OutputObjectAccessor>> CreateOutputObjectAccessorDictionaryAsync(ExecutionRequest execRequest)
        {
            var accessorDictionary = new Dictionary<string, OutputObjectAccessor>();

            foreach (var outputObjectName in execRequest.OutputObjects.Keys)
            {
                var outputObject = execRequest.OutputObjects[outputObjectName];

                var accessorRequest = new OutputObjectAccessorRequest
                {
                    ExecutionMetadata = execRequest,
                    ExpirationPeriod = execRequest.ExecutionTimeoutDuration,
                    ObjectMetadata = outputObject,
                    ObjectProviderName = execRequest.ObjectProviderName,
                    SignatureRsaKeyXml = execRequest.SignatureRsaKeyXml
                };

                var accessor = await outputObjectAccessorProvider.GetWritableAccessorAsync(accessorRequest);

                accessorDictionary[outputObjectName] = new OutputObjectAccessor(outputObject, accessor);
            }

            return accessorDictionary;
        }
    }
}
