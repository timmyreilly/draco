// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Models;
using Draco.Execution.Api.Interfaces;
using Draco.Execution.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Draco.Execution.Api.Services
{
    /// <summary>
    /// This is a convenience service to the execution API that pulls together all the object accessors
    /// needed by the client to upload input objects and download output objects.
    /// For more information, see /doc/architecture/execution-objects.md.
    /// </summary>
    public class ExtensionObjectApiModelService : IExtensionObjectApiModelService
    {
        private readonly IInputObjectAccessorProvider inputAccessorProvider;
        private readonly IOutputObjectAccessorProvider outputAccessorProvider;

        public ExtensionObjectApiModelService(
            IInputObjectAccessorProvider inputAccessorProvider,
            IOutputObjectAccessorProvider outputAccessorProvider)
        {
            this.inputAccessorProvider = inputAccessorProvider;
            this.outputAccessorProvider = outputAccessorProvider;
        }

        public async Task<Dictionary<string, InputObjectApiModel>> CreateInputObjectDictionaryAsync(
            IEnumerable<ExtensionInputObject> inputObjects, 
            Core.Models.Execution execution)
        {
            var dictionary = new Dictionary<string, InputObjectApiModel>();

            foreach (var inputObject in inputObjects)
            {
                var accessor = await inputAccessorProvider.GetWritableAccessorAsync(
                    new InputObjectAccessorRequest
                    {
                        ExecutionMetadata = execution,
                        ObjectMetadata = inputObject,
                        ObjectProviderName = execution.ObjectProviderName,
                        SignatureRsaKeyXml = execution.SignatureRsaKeyXml
                    });

                dictionary[inputObject.Name] =
                    new InputObjectApiModel
                    {
                        Accessor = accessor,
                        Description = inputObject.Description,
                        IsRequired = inputObject.IsRequired,
                        ObjectProviderName = execution.ObjectProviderName,
                        ObjectTypeName = inputObject.ObjectTypeName,
                        ObjectTypeUrl = inputObject.ObjectTypeUrl
                    };
            }

            return dictionary;
        }

        public async Task<Dictionary<string, OutputObjectApiModel>> CreateOutputObjectDictionaryAsync(
            IEnumerable<ExtensionOutputObject> outputObjects, 
            Core.Models.Execution execution)
        {
            var dictionary = new Dictionary<string, OutputObjectApiModel>();

            foreach (var outputObject in outputObjects)
            {
                var accessor = await outputAccessorProvider.GetReadableAccessorAsync(
                    new OutputObjectAccessorRequest
                    {
                        ExecutionMetadata = execution,
                        ObjectMetadata = outputObject,
                        ObjectProviderName = execution.ObjectProviderName,
                        SignatureRsaKeyXml = execution.SignatureRsaKeyXml 
                    });

                dictionary[outputObject.Name] =
                    new OutputObjectApiModel
                    {
                        Accessor = accessor,
                        Description = outputObject.Description,
                        ObjectProviderName = execution.ObjectProviderName,
                        ObjectTypeName = outputObject.ObjectTypeName,
                        ObjectTypeUrl = outputObject.ObjectTypeUrl 
                    };
            }

            return dictionary;
        }
    }
}
