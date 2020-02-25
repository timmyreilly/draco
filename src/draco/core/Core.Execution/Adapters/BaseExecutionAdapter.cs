// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Draco.Core.Execution.Adapters
{
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
