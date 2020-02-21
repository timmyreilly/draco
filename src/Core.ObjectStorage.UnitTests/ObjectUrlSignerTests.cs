// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Interfaces;
using Draco.Core.ObjectStorage.Models;
using Draco.Core.ObjectStorage.Services;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Draco.Core.ObjectStorage.UnitTests
{
    public class ObjectUrlSignerTests
    {
        [Fact]
        public void GenerateSignatureAsync_GivenNoRsaKeyXml_ShouldThrowArgumentNullException()
        {
            var mockSigner = new Mock<ISigner>();
            var objectUrlSignerUt = new ObjectUrlSigner(mockSigner.Object);

            Func<Task> act = async () => await objectUrlSignerUt.GenerateSignatureAsync(null, CreateTestObjectUrl());

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GenerateSignatureAsync_GivenNoObjectUrl_ShouldThrowArgumentNullException()
        {
            var mockSigner = new Mock<ISigner>();
            var objectUrlSignerUt = new ObjectUrlSigner(mockSigner.Object);

            Func<Task> act = async () => await objectUrlSignerUt.GenerateSignatureAsync("Some key.", null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task GenerateSignatureAsync_GivenHttpExecutionRequest_ShouldReturnSignature()
        {
            var mockSigner = new Mock<ISigner>();
            var objectUrlSignerUt = new ObjectUrlSigner(mockSigner.Object);
            var testRsaKeyXml = "Some key.";
            var testObjectUrl = CreateTestObjectUrl();
            var expectedSignature = "It worked!";

            mockSigner.Setup(s => s.GenerateSignatureAsync(testRsaKeyXml, testObjectUrl.Url))
                      .Returns(Task.FromResult(expectedSignature));

            var actualSignature = await objectUrlSignerUt.GenerateSignatureAsync(testRsaKeyXml, testObjectUrl);

            actualSignature.Should().Be(expectedSignature);
        }

        private ObjectUrl CreateTestObjectUrl() =>
            new ObjectUrl
            {
                ExpirationDateTimeUtc = DateTime.UtcNow.AddHours(1),
                Url = "http://some.url.com/"
            };
    }
}
