// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Models;
using Draco.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Draco.Core.Execution.Services
{
    /// <summary>
    /// Digitally signs HTTP execution requests.
    /// </summary>
    public class HttpExecutionRequestSigner : ISigner<HttpExecutionRequest>
    {
        private readonly ISigner stringSigner;

        public HttpExecutionRequestSigner(ISigner stringSigner)
        {
            this.stringSigner = stringSigner;
        }

        public Task<string> GenerateSignatureAsync(string rsaKeyXml, HttpExecutionRequest toSign)
        {
            if (string.IsNullOrEmpty(rsaKeyXml))
            {
                throw new ArgumentNullException(nameof(rsaKeyXml));
            }

            if (toSign == null)
            {
                throw new ArgumentNullException(nameof(toSign));
            }

            // Convert the execution request to a canonical string.
            // TODO: Document this string format so that target extensions can verify execution request signatures.

            var toSignAsString =
                $"{toSign.ExecutionId}|" +
                $"{toSign.ExecutionProfileName}|" +
                $"{toSign.ExtensionId}|" +
                $"{toSign.ExtensionVersionId}|" +
                $"{toSign.StatusUpdateKey}|" +
                $"{toSign.GetExecutionStatusUrl}|" +
                $"{toSign.UpdateExecutionStatusUrl}";

            // Use the generic string signer to sign the canonical execution request string.

            return stringSigner.GenerateSignatureAsync(rsaKeyXml, toSignAsString);
        }
    }
}
