// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Interfaces;
using Draco.Core.ObjectStorage.Models;
using System;
using System.Threading.Tasks;

namespace Draco.Core.ObjectStorage.Services
{
    public class ObjectUrlSigner : ISigner<ObjectUrl>
    {
        private readonly ISigner stringSigner;

        public ObjectUrlSigner(ISigner stringSigner)
        {
            this.stringSigner = stringSigner;
        }

        public Task<string> GenerateSignatureAsync(string rsaKeyXml, ObjectUrl toSign)
        {
            if (string.IsNullOrEmpty(rsaKeyXml))
            {
                throw new ArgumentNullException(nameof(rsaKeyXml));
            }

            if (toSign == null)
            {
                throw new ArgumentNullException(nameof(toSign));
            }

            return stringSigner.GenerateSignatureAsync(rsaKeyXml, toSign.Url);
        }
    }
}
