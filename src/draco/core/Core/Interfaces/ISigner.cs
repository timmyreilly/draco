// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;

namespace Draco.Core.Interfaces
{
    /// <summary>
    /// Defines a simple mechanism for digitally signing strings.
    /// </summary>
    public interface ISigner
    {
        Task<string> GenerateSignatureAsync(string rsaKeyXml, string toSign);
    }

    /// <summary>
    /// Defines a simple mechanism for digitally signing various types of objects.
    /// Typically, classes that implement this interface will first convert the object into a canonical string
    /// then sign that string using the non-generic form of this interface (above).
    /// </summary>
    /// <typeparam name="T">The type of object that this signer signs</typeparam>
    public interface ISigner<T>
    {
        Task<string> GenerateSignatureAsync(string rsaKeyXml, T toSign);
    }
}
