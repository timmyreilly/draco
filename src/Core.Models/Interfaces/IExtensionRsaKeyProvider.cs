// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Draco.Core.Models.Interfaces
{
    public interface IExtensionRsaKeyProvider
    {
        Task<string> GetExtensionRsaKeyXmlAsync(Extension extension);
    }
}
