// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Core.ObjectStorage.Providers
{
    public class StubInputObjectAccessorProvider : IInputObjectAccessorProvider // stub/v1: echoes back accessor request.
    {
        public Task<JObject> GetReadableAccessorAsync(InputObjectAccessorRequest accessorRequest)
        {
            if (accessorRequest == null)
            {
                throw new ArgumentNullException(nameof(accessorRequest));
            }

            return Task.FromResult(JObject.FromObject(accessorRequest));
        }

        public Task<JObject> GetWritableAccessorAsync(InputObjectAccessorRequest accessorRequest)
        {
            if (accessorRequest == null)
            {
                throw new ArgumentNullException(nameof(accessorRequest));
            }

            return Task.FromResult(JObject.FromObject(accessorRequest));
        }
    }
}
