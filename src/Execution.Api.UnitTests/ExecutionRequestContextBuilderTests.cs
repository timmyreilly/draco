using Draco.Core.Models;
using Draco.Core.Models.Enumerations;
using Draco.Core.Models.Interfaces;
using Draco.Execution.Api.Constants;
using Draco.Execution.Api.Interfaces;
using Draco.Execution.Api.Models;
using Draco.Execution.Api.Services;
using FluentAssertions;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Draco.Execution.Api.UnitTests
{
    public class ExecutionRequestContextBuilderTests
    {
        private const string DefaultExecutionModelName = "http/v1";
        private const string DefaultExecutionProfileName = "default";
        private const string DefaultObjectProviderName = "azure-blob/v1";

        private readonly JObject DefaultClientConfiguration = JObject.FromObject(
            new Dictionary<string, string>
            {
                ["Configuration A"] = "A",
                ["Configuration B"] = "B"
            });

        private readonly JObject DefaultExtensionSettings = JObject.FromObject(
            new Dictionary<string, string>
            {
                ["Setting A"] = "A",
                ["Setting B"] = "B"
            });

        private readonly string DefaultExtensionId = Guid.NewGuid().ToString();
        private readonly string DefaultExtensionVersionId = Guid.NewGuid().ToString();
        private readonly string DefaultUserId = Guid.NewGuid().ToString();
        private readonly string DefaultTenantId = Guid.NewGuid().ToString();

        [Fact]
        public async Task BuildExecutionRequestContextAsync_NoExtensionIdProvided_ShouldGenerateError()
        {
            var mockErContextValidator = new Mock<IExecutionRequestContextValidator>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();

            var erContextBuilder = new ExecutionRequestContextBuilder(
                mockErContextValidator.Object,
                mockExtensionRepository.Object);

            var execRequestApiModel = new ExecutionRequestApiModel
            {
                ExtensionVersionId = DefaultExtensionVersionId
            };

            var extension = CreateDefaultExtension();

            mockExtensionRepository
                .Setup(r => r.GetExtensionAsync(DefaultExtensionId))
                .Returns(Task.FromResult(extension));

            var erContext = await erContextBuilder.BuildExecutionRequestContextAsync(execRequestApiModel);

            erContext.ValidationErrors.Should().HaveCount(1);
            erContext.ValidationErrors.First().Should().Contain(ErrorCodes.ExtensionIdNotProvided);
        }

        [Fact]
        public async Task BuildExecutionRequestContextAsync_NoExtensionVersionIdProvided_ShouldGenerateError()
        {
            var mockErContextValidator = new Mock<IExecutionRequestContextValidator>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();

            var erContextBuilder = new ExecutionRequestContextBuilder(
                mockErContextValidator.Object,
                mockExtensionRepository.Object);

            var execRequestApiModel = new ExecutionRequestApiModel
            {
                ExtensionId = DefaultExtensionId
            };

            var extension = CreateDefaultExtension();

            mockExtensionRepository
                .Setup(r => r.GetExtensionAsync(DefaultExtensionId))
                .Returns(Task.FromResult(extension));

            var erContext = await erContextBuilder.BuildExecutionRequestContextAsync(execRequestApiModel);

            erContext.ValidationErrors.Should().HaveCount(1);
            erContext.ValidationErrors.First().Should().Contain(ErrorCodes.ExtensionVersionIdNotProvided);
        }

        [Fact]
        public async Task BuildExecutionRequestContextAsync_InvalidPriorityProvided_ShouldGenerateError()
        {
            var mockErContextValidator = new Mock<IExecutionRequestContextValidator>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();

            var erContextBuilder = new ExecutionRequestContextBuilder(
                mockErContextValidator.Object,
                mockExtensionRepository.Object);

            var execRequestApiModel = new ExecutionRequestApiModel
            {
                ExtensionId = DefaultExtensionId,
                ExtensionVersionId = DefaultExtensionVersionId,
                Priority = "Not a real priority."
            };

            var extension = CreateDefaultExtension();

            mockExtensionRepository
                .Setup(r => r.GetExtensionAsync(DefaultExtensionId))
                .Returns(Task.FromResult(extension));

            var erContext = await erContextBuilder.BuildExecutionRequestContextAsync(execRequestApiModel);

            erContext.ValidationErrors.Should().HaveCount(1);
            erContext.ValidationErrors.First().Should().Contain(ErrorCodes.InvalidPriority);
        }

        [Fact]
        public async Task BuildExecutionRequestContextAsync_InvalidExtensionIdProvided_ShouldGenerateError()
        {
            var mockErContextValidator = new Mock<IExecutionRequestContextValidator>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();

            var erContextBuilder = new ExecutionRequestContextBuilder(
                mockErContextValidator.Object,
                mockExtensionRepository.Object);

            var execRequestApiModel = new ExecutionRequestApiModel
            {
                ExtensionId = Guid.NewGuid().ToString(),
                ExtensionVersionId = DefaultExtensionVersionId
            };

            var extension = CreateDefaultExtension();

            mockExtensionRepository
                .Setup(r => r.GetExtensionAsync(DefaultExtensionId))
                .Returns(Task.FromResult(extension));

            var erContext = await erContextBuilder.BuildExecutionRequestContextAsync(execRequestApiModel);

            erContext.ValidationErrors.Should().HaveCount(1);
            erContext.ValidationErrors.First().Should().Contain(ErrorCodes.ExtensionNotFound);
        }

        [Fact]
        public async Task BuildExecutionRequestContextAsync_GivenADisabledExtension_ShouldGenerateError()
        {
            var mockErContextValidator = new Mock<IExecutionRequestContextValidator>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();

            var erContextBuilder = new ExecutionRequestContextBuilder(
                mockErContextValidator.Object,
                mockExtensionRepository.Object);

            var execRequestApiModel = new ExecutionRequestApiModel
            {
                ExtensionId = DefaultExtensionId,
                ExtensionVersionId = DefaultExtensionVersionId
            };

            var extension = CreateDefaultExtension();

            extension.IsActive = false;

            mockExtensionRepository
                .Setup(r => r.GetExtensionAsync(DefaultExtensionId))
                .Returns(Task.FromResult(extension));

            var erContext = await erContextBuilder.BuildExecutionRequestContextAsync(execRequestApiModel);

            erContext.ValidationErrors.Should().HaveCount(1);
            erContext.ValidationErrors.First().Should().Contain(ErrorCodes.ExtensionDisabled);
        }

        [Fact]
        public async Task BuildExecutionRequestContextAsync_InvalidExtensionVersionId_ShouldGenerateError()
        {
            var mockErContextValidator = new Mock<IExecutionRequestContextValidator>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();

            var erContextBuilder = new ExecutionRequestContextBuilder(
                mockErContextValidator.Object,
                mockExtensionRepository.Object);

            var execRequestApiModel = new ExecutionRequestApiModel
            {
                ExtensionId = DefaultExtensionId,
                ExtensionVersionId = Guid.NewGuid().ToString()
            };

            var extension = CreateDefaultExtension();

            mockExtensionRepository
                .Setup(r => r.GetExtensionAsync(DefaultExtensionId))
                .Returns(Task.FromResult(extension));

            var erContext = await erContextBuilder.BuildExecutionRequestContextAsync(execRequestApiModel);

            erContext.ValidationErrors.Should().HaveCount(1);
            erContext.ValidationErrors.First().Should().Contain(ErrorCodes.ExtensionVersionNotFound);
        }

        [Fact]
        public async Task BuildExecutionRequestContextAsync_GivenADisabledExtensionVersion_ShouldGenerateError()
        {
            var mockErContextValidator = new Mock<IExecutionRequestContextValidator>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();

            var erContextBuilder = new ExecutionRequestContextBuilder(
                mockErContextValidator.Object,
                mockExtensionRepository.Object);

            var execRequestApiModel = new ExecutionRequestApiModel
            {
                ExtensionId = DefaultExtensionId,
                ExtensionVersionId = DefaultExtensionVersionId
            };

            var extension = CreateDefaultExtension();

            extension.ExtensionVersions.First().IsActive = false;

            mockExtensionRepository
                .Setup(r => r.GetExtensionAsync(DefaultExtensionId))
                .Returns(Task.FromResult(extension));

            var erContext = await erContextBuilder.BuildExecutionRequestContextAsync(execRequestApiModel);

            erContext.ValidationErrors.Should().HaveCount(1);
            erContext.ValidationErrors.First().Should().Contain(ErrorCodes.ExtensionVersionDisabled);
        }

        [Fact]
        public async Task BuildExecutionRequestContextAsync_ValidateOnlyButExtensionVersionDoesNotSupportValidation_ShouldGenerateError()
        {
            var mockErContextValidator = new Mock<IExecutionRequestContextValidator>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();

            var erContextBuilder = new ExecutionRequestContextBuilder(
                mockErContextValidator.Object,
                mockExtensionRepository.Object);

            var execRequestApiModel = new ExecutionRequestApiModel
            {
                ExtensionId = DefaultExtensionId,
                ExtensionVersionId = DefaultExtensionVersionId,
                ValidateOnly = true
            };

            var extension = CreateDefaultExtension();

            extension.ExtensionVersions.First().SupportsValidation = false;

            mockExtensionRepository
                .Setup(r => r.GetExtensionAsync(DefaultExtensionId))
                .Returns(Task.FromResult(extension));

            var erContext = await erContextBuilder.BuildExecutionRequestContextAsync(execRequestApiModel);

            erContext.ValidationErrors.Should().HaveCount(1);
            erContext.ValidationErrors.First().Should().Contain(ErrorCodes.ExtensionVersionDoesNotSupportValidation);
        }

        [Fact]
        public async Task BuildExecutionRequestContextAsync_InvalidExecutionProfileName_ShouldGenerateError()
        {
            var mockErContextValidator = new Mock<IExecutionRequestContextValidator>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();

            var erContextBuilder = new ExecutionRequestContextBuilder(
                mockErContextValidator.Object,
                mockExtensionRepository.Object);

            var execRequestApiModel = new ExecutionRequestApiModel
            {
                ExtensionId = DefaultExtensionId,
                ExtensionVersionId = DefaultExtensionVersionId,
                ProfileName = "Not a real profile name."
            };

            var extension = CreateDefaultExtension();

            mockExtensionRepository
                .Setup(r => r.GetExtensionAsync(DefaultExtensionId))
                .Returns(Task.FromResult(extension));

            var erContext = await erContextBuilder.BuildExecutionRequestContextAsync(execRequestApiModel);

            erContext.ValidationErrors.Should().HaveCount(1);
            erContext.ValidationErrors.First().Should().Contain(ErrorCodes.ExecutionProfileNotFound);
        }

        [Fact]
        public async Task BuildExecutionRequestContextAsync_ExecutionProfileDoesNotSupportPriority_ShouldGenerateError()
        {
            var mockErContextValidator = new Mock<IExecutionRequestContextValidator>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();

            var erContextBuilder = new ExecutionRequestContextBuilder(
                mockErContextValidator.Object,
                mockExtensionRepository.Object);

            var execRequestApiModel = new ExecutionRequestApiModel
            {
                ExtensionId = DefaultExtensionId,
                ExtensionVersionId = DefaultExtensionVersionId,
                Priority = ExecutionPriority.Low.ToString()
            };

            var extension = CreateDefaultExtension();

            mockErContextValidator
                .Setup(v => v.ValidateExecutionRequestContextAsync(It.IsAny<IExecutionRequestContext>()))
                .Returns(Task.FromResult(true));

            mockExtensionRepository
                .Setup(r => r.GetExtensionAsync(DefaultExtensionId))
                .Returns(Task.FromResult(extension));

            var erContext = await erContextBuilder.BuildExecutionRequestContextAsync(execRequestApiModel);

            erContext.ValidationErrors.Should().HaveCount(1);
            erContext.ValidationErrors.First().Should().Contain(ErrorCodes.PriorityNotSupported);
        }

        [Fact]
        public async Task BuildExecutionRequestContextAsync_GivenMultipleMistakes_ShouldGenerateMultipleErrors()
        {
            var mockErContextValidator = new Mock<IExecutionRequestContextValidator>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();

            var erContextBuilder = new ExecutionRequestContextBuilder(
                mockErContextValidator.Object,
                mockExtensionRepository.Object);

            var execRequestApiModel = new ExecutionRequestApiModel
            {
                Priority = "Not a real priority."
            };

            var extension = CreateDefaultExtension();

            mockErContextValidator
                .Setup(v => v.ValidateExecutionRequestContextAsync(It.IsAny<IExecutionRequestContext>()))
                .Returns(Task.FromResult(true));

            mockExtensionRepository
                .Setup(r => r.GetExtensionAsync(DefaultExtensionId))
                .Returns(Task.FromResult(extension));

            var erContext = await erContextBuilder.BuildExecutionRequestContextAsync(execRequestApiModel);

            erContext.ValidationErrors.Should().HaveCount(3);
        }

        private Extension CreateDefaultExtension() =>
            new Extension
            {
                ExtensionId = DefaultExtensionId,
                IsActive = true,
                ExtensionVersions = new List<ExtensionVersion>
                {
                    CreateDefaultExtensionVersion()
                }
            };

        private ExtensionVersion CreateDefaultExtensionVersion() =>
            new ExtensionVersion
            {
                ExtensionId = DefaultExtensionId,
                ExtensionVersionId = DefaultExtensionVersionId,
                IsActive = true,
                IsLongRunning = true,
                SupportsValidation = true,
                ExecutionProfiles = new List<ExecutionProfile>
                {
                    CreateDefaultExecutionProfile()
                }
            };

        private ExecutionProfile CreateDefaultExecutionProfile() =>
            new ExecutionProfile
            {
                BaseExecutionCost = 1d,
                ClientConfiguration = DefaultClientConfiguration,
                ExecutionModelName = DefaultExecutionModelName,
                ExtensionSettings = DefaultExtensionSettings,
                IsActive = true,
                ObjectProviderName = DefaultObjectProviderName,
                ProfileName = DefaultExecutionProfileName,
                SupportedPriorities = ExecutionPriority.Normal | ExecutionPriority.High
            };
    }
}
