// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Draco.Core.Interfaces
{
    public interface ISigner
    {
        Task<string> GenerateSignatureAsync(string rsaKeyXml, string toSign);
    }

    public interface ISigner<T>
    {
        Task<string> GenerateSignatureAsync(string rsaKeyXml, T toSign);
    }
}
