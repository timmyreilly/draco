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
    public class CompositeInputObjectAccessorProviderTests
    {
        [Fact]
        public void GetReadableAccessorAsync_GivenNullInputObjectAccessorRequest_ShouldThrowException()
        {
            var mockServiceProvider = new Mock<IServiceProvider>();
            var accessorProviderFactory = new NamedServiceFactory<IInputObjectAccessorProvider>();

            var testAccessorProvider = new CompositeInputObjectAccessorProvider(
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
            var mockInputObjectAccessorProvider = new Mock<IInputObjectAccessorProvider>();

            var accessorProviderFactory = new NamedServiceFactory<IInputObjectAccessorProvider>
            {
                [expectedProviderName] = sp => mockInputObjectAccessorProvider.Object
            };

            var accessorRequest = new InputObjectAccessorRequest
            {
                ExecutionMetadata = new ExecutionRequest { ExecutionId = Guid.NewGuid().ToString() },
                ObjectMetadata = new ExtensionInputObject { Name = "InputObjectA" },
                ObjectProviderName = actualProviderName
            };

            var testAccessorProvider = new CompositeInputObjectAccessorProvider(
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
            var mockInputObjectAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var objectMetadata = new ExtensionInputObject { Name = "InputObjectA" };

            var accessorProviderFactory = new NamedServiceFactory<IInputObjectAccessorProvider>
            {
                [providerName] = sp => mockInputObjectAccessorProvider.Object
            };

            var accessorRequest = new InputObjectAccessorRequest
            {
                ExecutionMetadata = new ExecutionRequest { ExecutionId = Guid.NewGuid().ToString() },
                ObjectMetadata = objectMetadata,
                ObjectProviderName = providerName
            };

            var expectedAccessor = JObject.FromObject(new InputObjectAccessor
            {
                ObjectAccessor = JObject.FromObject(accessorRequest),
                ObjectMetadata = objectMetadata
            });

            mockInputObjectAccessorProvider.Setup(ap => ap.GetReadableAccessorAsync(accessorRequest))
                                           .Returns(Task.FromResult(expectedAccessor));

            var testAccessorProvider = new CompositeInputObjectAccessorProvider(
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
            var accessorProviderFactory = new NamedServiceFactory<IInputObjectAccessorProvider>();

            var testAccessorProvider = new CompositeInputObjectAccessorProvider(
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
            var mockInputObjectAccessorProvider = new Mock<IInputObjectAccessorProvider>();

            var accessorProviderFactory = new NamedServiceFactory<IInputObjectAccessorProvider>
            {
                [expectedProviderName] = sp => mockInputObjectAccessorProvider.Object
            };

            var accessorRequest = new InputObjectAccessorRequest
            {
                ExecutionMetadata = new ExecutionRequest { ExecutionId = Guid.NewGuid().ToString() },
                ObjectMetadata = new ExtensionInputObject { Name = "InputObjectA" },
                ObjectProviderName = actualProviderName
            };

            var testAccessorProvider = new CompositeInputObjectAccessorProvider(
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
            var mockInputObjectAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var objectMetadata = new ExtensionInputObject { Name = "InputObjectA" };

            var accessorProviderFactory = new NamedServiceFactory<IInputObjectAccessorProvider>
            {
                [providerName] = sp => mockInputObjectAccessorProvider.Object
            };

            var accessorRequest = new InputObjectAccessorRequest
            {
                ExecutionMetadata = new ExecutionRequest { ExecutionId = Guid.NewGuid().ToString() },
                ObjectMetadata = objectMetadata,
                ObjectProviderName = providerName
            };

            var expectedAccessor = JObject.FromObject(new InputObjectAccessor
            {
                ObjectAccessor = JObject.FromObject(accessorRequest),
                ObjectMetadata = objectMetadata
            });

            mockInputObjectAccessorProvider.Setup(ap => ap.GetWritableAccessorAsync(accessorRequest))
                                           .Returns(Task.FromResult(expectedAccessor));

            var testAccessorProvider = new CompositeInputObjectAccessorProvider(
                mockServiceProvider.Object,
                accessorProviderFactory);

            var actualAccessor = await testAccessorProvider.GetWritableAccessorAsync(accessorRequest);

            actualAccessor.Should().NotBeNull();
            actualAccessor.Should().BeEquivalentTo(expectedAccessor);
        }
    }
}
