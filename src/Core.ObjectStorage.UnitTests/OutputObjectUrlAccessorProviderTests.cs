// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Interfaces;
using Draco.Core.Models;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Models;
using Draco.Core.ObjectStorage.Providers;
using FluentAssertions;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Draco.Core.ObjectStorage.UnitTests
{
    public class OoutputObjectUrlAccessorProviderTests
    {
        [Fact]
        public void GetReadableAccessorAsync_GivenNullAccessorRequest_ShouldThrowException()
        {
            var mockUrlProvider = new Mock<IOutputObjectUrlProvider>();
            var mockSigner = new Mock<ISigner<ObjectUrl>>();

            var objectUrlAccessorProvider = new OutputObjectUrlAccessorProvider(mockUrlProvider.Object, mockSigner.Object);

            Func<Task> act = async () => await objectUrlAccessorProvider.GetReadableAccessorAsync(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetWritableAccessorAsync_GivenNullAccessorRequest_ShouldThrowException()
        {
            var mockUrlProvider = new Mock<IOutputObjectUrlProvider>();
            var mockSigner = new Mock<ISigner<ObjectUrl>>();

            var objectUrlAccessorProvider = new OutputObjectUrlAccessorProvider(mockUrlProvider.Object, mockSigner.Object);

            Func<Task> act = async () => await objectUrlAccessorProvider.GetWritableAccessorAsync(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task GetReadableAccessorAsync_GivenValidAccessorRequest_ShouldReturnAccessor()
        {
            var mockUrlProvider = new Mock<IOutputObjectUrlProvider>();
            var mockSigner = new Mock<ISigner<ObjectUrl>>();

            var objectUrlAccessorProvider = new OutputObjectUrlAccessorProvider(mockUrlProvider.Object, mockSigner.Object);

            var accessorRequest = new OutputObjectAccessorRequest
            {
                ExecutionMetadata = new ExecutionRequest { ExecutionId = Guid.NewGuid().ToString() },
                ObjectMetadata = new ExtensionOutputObject { Name = "OutputObjectA" }
            };

            var objectUrl = new ObjectUrl
            {
                Url = "http://test.com/some-object",
                HttpMethod = HttpMethod.Get.Method
            };

            var objectUrlJson = JObject.FromObject(objectUrl);

            mockUrlProvider.Setup(up => up.GetReadableUrlAsync(It.Is<ObjectUrlRequest>(
                                  ur => ur.ExecutionMetadata.ExecutionId == accessorRequest.ExecutionMetadata.ExecutionId &&
                                        ur.ObjectName == accessorRequest.ObjectMetadata.Name)))
                           .Returns(Task.FromResult(objectUrl));

            var accessor = await objectUrlAccessorProvider.GetReadableAccessorAsync(accessorRequest);

            accessor.Should().NotBeNull();
            accessor.Should().BeEquivalentTo(objectUrlJson);
        }

        [Fact]
        public async Task GetWritableAccessorAsync_GivenValidAccessorRequest_ShouldReturnAccessor()
        {
            var mockUrlProvider = new Mock<IOutputObjectUrlProvider>();
            var mockSigner = new Mock<ISigner<ObjectUrl>>();

            var objectUrlAccessorProvider = new OutputObjectUrlAccessorProvider(mockUrlProvider.Object, mockSigner.Object);

            var accessorRequest = new OutputObjectAccessorRequest
            {
                ExecutionMetadata = new ExecutionRequest { ExecutionId = Guid.NewGuid().ToString() },
                ObjectMetadata = new ExtensionOutputObject { Name = "OutputObjectA" }
            };

            var objectUrl = new ObjectUrl
            {
                Url = "http://test.com/some-object",
                HttpMethod = HttpMethod.Put.Method
            };

            var objectUrlJson = JObject.FromObject(objectUrl);

            mockUrlProvider.Setup(up => up.GetWritableUrlAsync(It.Is<ObjectUrlRequest>(
                                  ur => ur.ExecutionMetadata.ExecutionId == accessorRequest.ExecutionMetadata.ExecutionId &&
                                        ur.ObjectName == accessorRequest.ObjectMetadata.Name)))
                           .Returns(Task.FromResult(objectUrl));

            var accessor = await objectUrlAccessorProvider.GetWritableAccessorAsync(accessorRequest);

            accessor.Should().NotBeNull();
            accessor.Should().BeEquivalentTo(objectUrlJson);
        }
    }
}
