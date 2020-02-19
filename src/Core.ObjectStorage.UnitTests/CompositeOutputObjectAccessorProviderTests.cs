// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Factories;
using Draco.Core.Models;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Models;
using Draco.Core.ObjectStorage.Providers;
using FluentAssertions;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Draco.Core.ObjectStorage.UnitTests
{
    public class CompositeOutputObjectAccessorProviderTests
    {
        [Fact]
        public void GetReadableAccessorAsync_GivenNullOutputObjectAccessorRequest_ShouldThrowException()
        {
            var mockServiceProvider = new Mock<IServiceProvider>();
            var accessorProviderFactory = new NamedServiceFactory<IOutputObjectAccessorProvider>();

            var testAccessorProvider = new CompositeOutputObjectAccessorProvider(
                mockServiceProvider.Object,
                accessorProviderFactory);

            Func<Task> act = async () => await testAccessorProvider.GetReadableAccessorAsync(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetReadableAccessorAsync_GivenUnsupportedObjectProviderName_ShouldThrowException()
        {
            var expectedProviderName = "expected-provider";
            var actualProviderName = "actual-provider";

            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockObjectAccessorProvider = new Mock<IOutputObjectAccessorProvider>();

            var accessorProviderFactory = new NamedServiceFactory<IOutputObjectAccessorProvider>
            {
                [expectedProviderName] = sp => mockObjectAccessorProvider.Object
            };

            var accessorRequest = new OutputObjectAccessorRequest
            {
                ExecutionMetadata = new ExecutionRequest { ExecutionId = Guid.NewGuid().ToString() },
                ObjectMetadata = new ExtensionOutputObject { Name = "OutputObjectA" },
                ObjectProviderName = actualProviderName
            };

            var testAccessorProvider = new CompositeOutputObjectAccessorProvider(
                mockServiceProvider.Object,
                accessorProviderFactory);

            Func<Task> act = async () => await testAccessorProvider.GetReadableAccessorAsync(accessorRequest);

            act.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public async Task GetReadableAccessorAsync_GivenSupportedObjectProviderName_ShouldReturnAccessor()
        {
            var providerName = "provider";

            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockObjectAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var objectMetadata = new ExtensionOutputObject { Name = "OutputObjectA" };

            var accessorProviderFactory = new NamedServiceFactory<IOutputObjectAccessorProvider>
            {
                [providerName] = sp => mockObjectAccessorProvider.Object
            };

            var accessorRequest = new OutputObjectAccessorRequest
            {
                ExecutionMetadata = new ExecutionRequest { ExecutionId = Guid.NewGuid().ToString() },
                ObjectMetadata = objectMetadata,
                ObjectProviderName = providerName
            };

            var expectedAccessor = JObject.FromObject(new OutputObjectAccessor
            {
                ObjectAccessor = JObject.FromObject(accessorRequest),
                ObjectMetadata = objectMetadata
            });

            mockObjectAccessorProvider.Setup(ap => ap.GetReadableAccessorAsync(accessorRequest))
                                           .Returns(Task.FromResult(expectedAccessor));

            var testAccessorProvider = new CompositeOutputObjectAccessorProvider(
                mockServiceProvider.Object,
                accessorProviderFactory);

            var actualAccessor = await testAccessorProvider.GetReadableAccessorAsync(accessorRequest);

            actualAccessor.Should().NotBeNull();
            actualAccessor.Should().BeEquivalentTo(expectedAccessor);
        }

        [Fact]
        public void GetWritableAccessorAsync_GivenNullInputObjectAccessorRequest_ShouldThrowException()
        {
            var mockServiceProvider = new Mock<IServiceProvider>();
            var accessorProviderFactory = new NamedServiceFactory<IOutputObjectAccessorProvider>();

            var testAccessorProvider = new CompositeOutputObjectAccessorProvider(
                mockServiceProvider.Object,
                accessorProviderFactory);

            Func<Task> act = async () => await testAccessorProvider.GetWritableAccessorAsync(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetWritableAccessorAsync_GivenUnsupportedObjectProviderName_ShouldThrowException()
        {
            var expectedProviderName = "expected-provider";
            var actualProviderName = "actual-provider";

            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockObjectAccessorProvider = new Mock<IOutputObjectAccessorProvider>();

            var accessorProviderFactory = new NamedServiceFactory<IOutputObjectAccessorProvider>
            {
                [expectedProviderName] = sp => mockObjectAccessorProvider.Object
            };

            var accessorRequest = new OutputObjectAccessorRequest
            {
                ExecutionMetadata = new ExecutionRequest { ExecutionId = Guid.NewGuid().ToString() },
                ObjectMetadata = new ExtensionOutputObject { Name = "OutputObjectA" },
                ObjectProviderName = actualProviderName
            };

            var testAccessorProvider = new CompositeOutputObjectAccessorProvider(
                mockServiceProvider.Object,
                accessorProviderFactory);

            Func<Task> act = async () => await testAccessorProvider.GetWritableAccessorAsync(accessorRequest);

            act.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public async Task GetWritableAccessorAsync_GivenSupportedObjectProviderName_ShouldReturnAccessor()
        {
            var providerName = "provider";

            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockObjectAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var objectMetadata = new ExtensionOutputObject { Name = "OutputObjectA" };

            var accessorProviderFactory = new NamedServiceFactory<IOutputObjectAccessorProvider>
            {
                [providerName] = sp => mockObjectAccessorProvider.Object
            };

            var accessorRequest = new OutputObjectAccessorRequest
            {
                ExecutionMetadata = new ExecutionRequest { ExecutionId = Guid.NewGuid().ToString() },
                ObjectMetadata = objectMetadata,
                ObjectProviderName = providerName
            };

            var expectedAccessor = JObject.FromObject(new OutputObjectAccessor
            {
                ObjectAccessor = JObject.FromObject(accessorRequest),
                ObjectMetadata = objectMetadata
            });

            mockObjectAccessorProvider.Setup(ap => ap.GetWritableAccessorAsync(accessorRequest))
                                      .Returns(Task.FromResult(expectedAccessor));

            var testAccessorProvider = new CompositeOutputObjectAccessorProvider(
                mockServiceProvider.Object,
                accessorProviderFactory);

            var actualAccessor = await testAccessorProvider.GetWritableAccessorAsync(accessorRequest);

            actualAccessor.Should().NotBeNull();
            actualAccessor.Should().BeEquivalentTo(expectedAccessor);
        }
    }
}
