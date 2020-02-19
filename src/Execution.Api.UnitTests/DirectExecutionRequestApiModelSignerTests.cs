// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Interfaces;
using Draco.Execution.Api.Models;
using Draco.Execution.Api.Services;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Draco.Execution.Api.UnitTests
{
    public class DirectExecutionRequestApiModelSignerTests
    {
        [Fact]
        public void GenerateSignatureAsync_GivenNoRsaKeyXml_ShouldThrowArgumentNullException()
        {
            var mockSigner = new Mock<ISigner>();
            var signerUt = new DirectExecutionRequestApiModelSigner(mockSigner.Object);

            Func<Task> act = async () => await signerUt.GenerateSignatureAsync(null, CreateTestApiModel());

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GenerateSignatureAsync_GivenNoApiModel_ShouldThrowArgumentNullException()
        {
            var mockSigner = new Mock<ISigner>();
            var signerUt = new DirectExecutionRequestApiModelSigner(mockSigner.Object);

            Func<Task> act = async () => await signerUt.GenerateSignatureAsync("Some key.", null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task GenerateSignatureAsync_GivenDirectExecutionRequestApiModel_ShouldReturnSignature()
        {
            var mockSigner = new Mock<ISigner>();
            var signerUt = new DirectExecutionRequestApiModelSigner(mockSigner.Object);
            var testRsaKeyXml = "Some key.";
            var testApiModel = CreateTestApiModel();
            var testApiModelString = ToCanonicalString(testApiModel);
            var expectedSignature = "It worked!";

            mockSigner.Setup(s => s.GenerateSignatureAsync(testRsaKeyXml, testApiModelString))
                      .Returns(Task.FromResult(expectedSignature));

            var actualSignature = await signerUt.GenerateSignatureAsync(testRsaKeyXml, testApiModel);

            actualSignature.Should().Be(expectedSignature);
        }

        private DirectExecutionRequestApiModel CreateTestApiModel() =>
            new DirectExecutionRequestApiModel
            {
                ExecutionId = Guid.NewGuid().ToString(),
                ExecutionModelName = "default",
                ExecutionProfileName = "default",
                ExpirationDateTimeUtc = DateTime.UtcNow.AddHours(1),
                ExtensionId = Guid.NewGuid().ToString(),
                ExtensionVersionId = Guid.NewGuid().ToString(),
                GetExecutionStatusUrl = "http://some.url.com/",
                ObjectProviderName = "default"
            };

        private string ToCanonicalString(DirectExecutionRequestApiModel apiModel) =>
            $"{apiModel.ExecutionId}|" +
            $"{apiModel.ExtensionId}|" +
            $"{apiModel.ExtensionVersionId}|" +
            $"{apiModel.ExecutionModelName}|" +
            $"{apiModel.ExecutionProfileName}|" +
            $"{apiModel.ObjectProviderName}|" +
            $"{apiModel.GetExecutionStatusUrl}|" +
            $"{apiModel.ExpirationDateTimeUtc.ToString("s")}";
    }
}
