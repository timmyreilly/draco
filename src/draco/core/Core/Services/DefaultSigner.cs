// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Draco.Core.Services
{
    /// <summary>
    /// A simple mechanism for digitally signing strings.
    /// </summary>
    public class DefaultSigner : ISigner
    {
        /// <summary>
        /// Generate a digital signature for a string.
        /// </summary>
        /// <param name="rsaKeyXml">The XML-formatted RSA key material</param>
        /// <param name="toSign">The string to sign</param>
        /// <returns></returns>
        public Task<string> GenerateSignatureAsync(string rsaKeyXml, string toSign)
        {
            if (string.IsNullOrEmpty(rsaKeyXml))
            {
                throw new ArgumentNullException(nameof(rsaKeyXml));
            }

            if (string.IsNullOrEmpty(toSign))
            {
                throw new ArgumentNullException(nameof(toSign));
            }
    
            using (var shaProvider = SHA512.Create())
            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                rsaProvider.FromXmlString(rsaKeyXml);

                var toSignBytes = Encoding.UTF8.GetBytes(toSign);
                var toSignHashBytes = shaProvider.ComputeHash(toSignBytes);
                var rsaFormatter = new RSAPKCS1SignatureFormatter(rsaProvider);

                rsaFormatter.SetHashAlgorithm("SHA512");

                var signatureBytes = rsaFormatter.CreateSignature(toSignHashBytes);

                return Task.FromResult(Convert.ToBase64String(signatureBytes));
            }
        }
    }
}
