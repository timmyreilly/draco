using Draco.Core.Interfaces;
using Draco.Execution.Api.Models;
using System;
using System.Threading.Tasks;

namespace Draco.Execution.Api.Services
{
    public class DirectExecutionRequestApiModelSigner : ISigner<DirectExecutionRequestApiModel>
    {
        private readonly ISigner stringSigner;

        public DirectExecutionRequestApiModelSigner(ISigner stringSigner)
        {
            this.stringSigner = stringSigner;
        }

        public Task<string> GenerateSignatureAsync(string rsaKeyXml, DirectExecutionRequestApiModel toSign)
        {
            if (string.IsNullOrEmpty(rsaKeyXml))
            {
                throw new ArgumentNullException(nameof(rsaKeyXml));
            }

            if (toSign == null)
            {
                throw new ArgumentNullException(nameof(toSign));
            }

            var apiModelString =
                $"{toSign.ExecutionId}|" +
                $"{toSign.ExtensionId}|" +
                $"{toSign.ExtensionVersionId}|" +
                $"{toSign.ExecutionModelName}|" +
                $"{toSign.ExecutionProfileName}|" +
                $"{toSign.ObjectProviderName}|" +
                $"{toSign.GetExecutionStatusUrl}|" +
                $"{toSign.ExpirationDateTimeUtc.ToString("s")}";

            return stringSigner.GenerateSignatureAsync(rsaKeyXml, apiModelString);
        }
    }
}
