// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Models;
using Draco.Core.Execution.Services;
using Draco.Core.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Draco.Core.Execution.UnitTests
{
    public class HttpExecutionRequestSignerTests
    {
        [Fact]
        public void GenerateSignatureAsync_GivenNoRsaKeyXml_ShouldThrowArgumentNullException()
        {
            var mockSigner = new Mock<ISigner>();
            var httpExecRequestSignerUt = new HttpExecutionRequestSigner(mockSigner.Object);
            var testExecRequest = CreateTestHttpExecutionRequest();

            Func<Task> act = async () => await httpExecRequestSignerUt.GenerateSignatureAsync(null, testExecRequest);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GenerateSignatureAsync_GivenNoHttpExecutionRequest_ShouldThrowArgumentNullException()
        {
            var mockSigner = new Mock<ISigner>();
            var httpExecRequestSignerUt = new HttpExecutionRequestSigner(mockSigner.Object);

            Func<Task> act = async () => await httpExecRequestSignerUt.GenerateSignatureAsync("Some key.", null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task GenerateSignatureAsync_GivenHttpExecutionRequest_ShouldReturnSignature()
        {
            var mockSigner = new Mock<ISigner>();
            var httpExecRequestSignerUt = new HttpExecutionRequestSigner(mockSigner.Object);
            var testRsaKeyXml = "Some key.";
            var testExecRequest = CreateTestHttpExecutionRequest();
            var testExecRequestString = ToCanonicalString(testExecRequest);
            var expectedSignature = "It worked!";

            mockSigner.Setup(s => s.GenerateSignatureAsync(testRsaKeyXml, testExecRequestString))
                      .Returns(Task.FromResult(expectedSignature));

            var actualSignature = await httpExecRequestSignerUt.GenerateSignatureAsync(testRsaKeyXml, testExecRequest);

            actualSignature.Should().Be(expectedSignature);
        }

        private HttpExecutionRequest CreateTestHttpExecutionRequest() =>
            new HttpExecutionRequest
            {
                ExecutionId = Guid.NewGuid().ToString(),
                ExecutionProfileName = "default",
                ExtensionId = Guid.NewGuid().ToString(),
                ExtensionVersionId = Guid.NewGuid().ToString(),
                StatusUpdateKey = Guid.NewGuid().ToString(),
                GetExecutionStatusUrl = "https://some.url.com/",
                UpdateExecutionStatusUrl = "https://some.otherurl.com/"
            };

        private string ToCanonicalString(HttpExecutionRequest httpExecRequest) =>
             $"{httpExecRequest.ExecutionId}|" +
             $"{httpExecRequest.ExecutionProfileName}|" +
             $"{httpExecRequest.ExtensionId}|" +
             $"{httpExecRequest.ExtensionVersionId}|" +
             $"{httpExecRequest.StatusUpdateKey}|" +
             $"{httpExecRequest.GetExecutionStatusUrl}|" +
             $"{httpExecRequest.UpdateExecutionStatusUrl}";
    }
}
