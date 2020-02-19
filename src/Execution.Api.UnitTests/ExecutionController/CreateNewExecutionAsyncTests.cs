// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Interfaces;
using Draco.Core.Interfaces;
using Draco.Core.Models;
using Draco.Core.Models.Enumerations;
using Draco.Core.Models.Interfaces;
using Draco.Core.ObjectStorage.Models;
using Draco.Core.Services.Interfaces;
using Draco.Execution.Api.Interfaces;
using Draco.Execution.Api.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Draco.Execution.Api.UnitTests.ExecutionController
{
    public class CreateNewExecutionAsyncTests
    {
        private const string DefaultExecutionModelName = "http/v1";
        private const string DefaultExecutionProfileName = "default";
        private const string DefaultObjectProviderName = "azure-blob/v1";
        private const string DefaultExtensionRsaKey = "Hi! I'm an RSA key!";
        private const string DefaultExtensionSignature = "Howdy! I'm digitally signed so you know you can trust me!";

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

        private readonly JObject DefaultServiceConfiguration = JObject.FromObject(
            new Dictionary<string, string>
            {
                ["Service A"] = "A",
                ["Service B"] = "B"
            });

        private readonly string DefaultExtensionId = Guid.NewGuid().ToString();
        private readonly string DefaultExtensionVersionId = Guid.NewGuid().ToString();
        private readonly string DefaultUserId = Guid.NewGuid().ToString();
        private readonly string DefaultTenantId = Guid.NewGuid().ToString();

        [Fact]
        public async Task CreateNewExecutionAsync_OnValidationErrors_ShouldRespondBadRequest()
        {
            var mockErContextBuilder = new Mock<IExecutionRequestContextBuilder>();
            var mockExecRepository = new Mock<IExecutionRepository>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();
            var mockExtensionObjectApiModelService = new Mock<IExtensionObjectApiModelService>();
            var mockExtensionRsaKeyProvider = new Mock<IExtensionRsaKeyProvider>();
            var mockExtensionSettingsBuilder = new Mock<IExtensionSettingsBuilder>();
            var mockExecRequestRouter = new Mock<IExecutionRequestRouter>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockExecUpdatePublisher = new Mock<IExecutionUpdatePublisher>();
            var mockDirectExecRequestSigner = new Mock<ISigner<DirectExecutionRequestApiModel>>();
            var userContext = CreateDefaultUserContext();

            var execController = new Controllers.ExecutionController(
                mockErContextBuilder.Object,
                mockExecRepository.Object,
                mockExtensionRepository.Object,
                mockExtensionObjectApiModelService.Object,
                mockExtensionRsaKeyProvider.Object,
                mockExtensionSettingsBuilder.Object,
                mockExecRequestRouter.Object,
                mockExecUpdatePublisher.Object,
                mockExecServiceProvider.Object,
                mockDirectExecRequestSigner.Object,
                userContext);

            var execRequestApiModel = new ExecutionRequestApiModel
            {
                ExtensionId = DefaultExtensionId,
                ExtensionVersionId = DefaultExtensionVersionId
            };

            var erContext = new ExecutionRequestContext<ExecutionRequestApiModel>
            {
                OriginalRequest = execRequestApiModel,
                ValidationErrors = new List<string>
                {
                    "You did it wrong!"
                }
            };

            mockErContextBuilder.Setup(b => b.BuildExecutionRequestContextAsync(execRequestApiModel))
                                .Returns(Task.FromResult(erContext));

            var apiResponse = await execController.CreateNewExecutionAsync(execRequestApiModel) as BadRequestObjectResult;

            apiResponse.Should().NotBeNull();
            apiResponse.Value.Should().BeOfType<string>();
            apiResponse.Value.ToString().Should().Contain(erContext.ValidationErrors.First());
        }

        [Fact]
        public async Task CreateNewExecutionAsync_GivenExtensionVersionWithInputObjects_ShouldRespondAccepted()
        {
            var mockErContextBuilder = new Mock<IExecutionRequestContextBuilder>();
            var mockExecRepository = new Mock<IExecutionRepository>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();
            var mockExtensionObjectApiModelService = new Mock<IExtensionObjectApiModelService>();
            var mockExtensionRsaKeyProvider = new Mock<IExtensionRsaKeyProvider>();
            var mockExtensionSettingsBuilder = new Mock<IExtensionSettingsBuilder>();
            var mockExecRequestRouter = new Mock<IExecutionRequestRouter>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockExecUpdatePublisher = new Mock<IExecutionUpdatePublisher>();
            var mockDirectExecRequestSigner = new Mock<ISigner<DirectExecutionRequestApiModel>>();
            var userContext = CreateDefaultUserContext();

            var execController = new Controllers.ExecutionController(
                mockErContextBuilder.Object,
                mockExecRepository.Object,
                mockExtensionRepository.Object,
                mockExtensionObjectApiModelService.Object,
                mockExtensionRsaKeyProvider.Object,
                mockExtensionSettingsBuilder.Object,
                mockExecRequestRouter.Object,
                mockExecUpdatePublisher.Object,
                mockExecServiceProvider.Object,
                mockDirectExecRequestSigner.Object,
                userContext);

            var execRequestApiModel =
                new ExecutionRequestApiModel
                {
                    ExtensionId = DefaultExtensionId,
                    ExtensionVersionId = DefaultExtensionVersionId
                };

            var erContext =
                new ExecutionRequestContext<ExecutionRequestApiModel>
                {
                    OriginalRequest = execRequestApiModel,
                    Extension = CreateDefaultExtension(),
                    ExtensionVersion = CreateDefaultExtensionVersion(),
                    ExecutionProfile = CreateDefaultExecutionProfile()
                };

            var inputObject = CreateDefaultExtensionInputObject();

            erContext.ExtensionVersion.InputObjects.Add(inputObject);

            mockErContextBuilder
                .Setup(b => b.BuildExecutionRequestContextAsync(execRequestApiModel))
                .Returns(Task.FromResult(erContext));

            var inputObjectApiModel = CreateDefaultInputObjectApiModel();

            mockExtensionObjectApiModelService
                .Setup(s => s.CreateInputObjectDictionaryAsync(erContext.ExtensionVersion.InputObjects, It.IsAny<Core.Models.Execution>()))
                .Returns(Task.FromResult(new Dictionary<string, InputObjectApiModel> { [inputObject.Name] = inputObjectApiModel }));

            var apiResponse = await execController.CreateNewExecutionAsync(execRequestApiModel) as AcceptedResult;

            apiResponse.Should().NotBeNull();
            apiResponse.Value.Should().NotBeNull();
            apiResponse.Value.Should().BeOfType<ToContinueExecutionApiModel>();

            var toContinueApiModel = apiResponse.Value as ToContinueExecutionApiModel;

            toContinueApiModel.ExecutionStatus.Should().Be(ExecutionStatus.PendingInputObjects.ToString());
            toContinueApiModel.InputObjects.Should().ContainKey(inputObject.Name);
            toContinueApiModel.InputObjects[inputObject.Name].Should().Be(inputObjectApiModel);
        }

        [Fact]
        public async Task CreateNewExecutionAsync_GivenExtensionVersionWithInputObjects_ShouldSaveExecution()
        {
            var mockErContextBuilder = new Mock<IExecutionRequestContextBuilder>();
            var mockExecRepository = new Mock<IExecutionRepository>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();
            var mockExtensionObjectApiModelService = new Mock<IExtensionObjectApiModelService>();
            var mockExtensionRsaKeyProvider = new Mock<IExtensionRsaKeyProvider>();
            var mockExtensionSettingsBuilder = new Mock<IExtensionSettingsBuilder>();
            var mockExecRequestRouter = new Mock<IExecutionRequestRouter>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockExecUpdatePublisher = new Mock<IExecutionUpdatePublisher>();
            var mockDirectExecRequestSigner = new Mock<ISigner<DirectExecutionRequestApiModel>>();
            var userContext = CreateDefaultUserContext();

            var execController = new Controllers.ExecutionController(
                mockErContextBuilder.Object,
                mockExecRepository.Object,
                mockExtensionRepository.Object,
                mockExtensionObjectApiModelService.Object,
                mockExtensionRsaKeyProvider.Object,
                mockExtensionSettingsBuilder.Object,
                mockExecRequestRouter.Object,
                mockExecUpdatePublisher.Object,
                mockExecServiceProvider.Object,
                mockDirectExecRequestSigner.Object,
                userContext);

            var execRequestApiModel =
                new ExecutionRequestApiModel
                {
                    ExtensionId = DefaultExtensionId,
                    ExtensionVersionId = DefaultExtensionVersionId
                };

            var erContext =
                new ExecutionRequestContext<ExecutionRequestApiModel>
                {
                    OriginalRequest = execRequestApiModel,
                    Extension = CreateDefaultExtension(),
                    ExtensionVersion = CreateDefaultExtensionVersion(),
                    ExecutionProfile = CreateDefaultExecutionProfile()
                };

            var inputObject = CreateDefaultExtensionInputObject();

            erContext.ExtensionVersion.InputObjects.Add(inputObject);

            mockErContextBuilder
                .Setup(b => b.BuildExecutionRequestContextAsync(execRequestApiModel))
                .Returns(Task.FromResult(erContext));

            var inputObjectApiModel = CreateDefaultInputObjectApiModel();

            mockExtensionObjectApiModelService
                .Setup(s => s.CreateInputObjectDictionaryAsync(erContext.ExtensionVersion.InputObjects, It.IsAny<Core.Models.Execution>()))
                .Returns(Task.FromResult(new Dictionary<string, InputObjectApiModel> { [inputObject.Name] = inputObjectApiModel }));

            Core.Models.Execution execution = null;

            mockExecRepository
                .Setup(r => r.UpsertExecutionAsync(It.IsAny<Core.Models.Execution>()))
                .Callback<Core.Models.Execution>(e => execution = e);

            await execController.CreateNewExecutionAsync(execRequestApiModel);

            execution.Should().NotBeNull();
            execution.ExecutionModelName.Should().Be(erContext.ExecutionProfile.ExecutionModelName);
            execution.ExecutionProfileName.Should().Be(erContext.ExecutionProfile.ProfileName);
            execution.Executor.Should().NotBeNull();
            execution.ExtensionId.Should().Be(erContext.Extension.ExtensionId);
            execution.ExtensionVersionId.Should().Be(erContext.ExtensionVersion.ExtensionVersionId);
            execution.InputObjects.Should().HaveCount(1);
            execution.InputObjects.First().Description.Should().Be(inputObject.Description);
            execution.InputObjects.First().IsRequired.Should().Be(inputObject.IsRequired);
            execution.InputObjects.First().Name.Should().Be(inputObject.Name);
            execution.Mode.Should().Be(erContext.ExecutionProfile.ExecutionMode);
            execution.ObjectProviderName.Should().Be(erContext.ExecutionProfile.ObjectProviderName);
            execution.Priority.Should().Be(Enum.Parse<ExecutionPriority>(erContext.OriginalRequest.Priority));
            execution.Status.Should().Be(ExecutionStatus.PendingInputObjects);
        }

        [Fact]
        public async Task CreateNewExecutionAsync_GivenExtensionWithInputObjects_ShouldPublishStatusUpdateEvent()
        {
            var mockErContextBuilder = new Mock<IExecutionRequestContextBuilder>();
            var mockExecRepository = new Mock<IExecutionRepository>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();
            var mockExtensionObjectApiModelService = new Mock<IExtensionObjectApiModelService>();
            var mockExtensionRsaKeyProvider = new Mock<IExtensionRsaKeyProvider>();
            var mockExtensionSettingsBuilder = new Mock<IExtensionSettingsBuilder>();
            var mockExecRequestRouter = new Mock<IExecutionRequestRouter>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockExecUpdatePublisher = new Mock<IExecutionUpdatePublisher>();
            var mockDirectExecRequestSigner = new Mock<ISigner<DirectExecutionRequestApiModel>>();
            var userContext = CreateDefaultUserContext();

            var execController = new Controllers.ExecutionController(
                mockErContextBuilder.Object,
                mockExecRepository.Object,
                mockExtensionRepository.Object,
                mockExtensionObjectApiModelService.Object,
                mockExtensionRsaKeyProvider.Object,
                mockExtensionSettingsBuilder.Object,
                mockExecRequestRouter.Object,
                mockExecUpdatePublisher.Object,
                mockExecServiceProvider.Object,
                mockDirectExecRequestSigner.Object,
                userContext);

            var execRequestApiModel =
                new ExecutionRequestApiModel
                {
                    ExtensionId = DefaultExtensionId,
                    ExtensionVersionId = DefaultExtensionVersionId
                };

            var erContext =
                new ExecutionRequestContext<ExecutionRequestApiModel>
                {
                    OriginalRequest = execRequestApiModel,
                    Extension = CreateDefaultExtension(),
                    ExtensionVersion = CreateDefaultExtensionVersion(),
                    ExecutionProfile = CreateDefaultExecutionProfile()
                };

            var inputObject = CreateDefaultExtensionInputObject();

            erContext.ExtensionVersion.InputObjects.Add(inputObject);

            mockErContextBuilder
                .Setup(b => b.BuildExecutionRequestContextAsync(execRequestApiModel))
                .Returns(Task.FromResult(erContext));

            var inputObjectApiModel = CreateDefaultInputObjectApiModel();

            mockExtensionObjectApiModelService
                .Setup(s => s.CreateInputObjectDictionaryAsync(erContext.ExtensionVersion.InputObjects, It.IsAny<Core.Models.Execution>()))
                .Returns(Task.FromResult(new Dictionary<string, InputObjectApiModel> { [inputObject.Name] = inputObjectApiModel }));

            Core.Models.Execution execution = null;

            mockExecUpdatePublisher
                .Setup(p => p.PublishUpdateAsync(It.IsAny<Core.Models.Execution>()))
                .Callback<Core.Models.Execution>(e => execution = e);

            await execController.CreateNewExecutionAsync(execRequestApiModel);

            execution.Should().NotBeNull();
            execution.ExecutionModelName.Should().Be(erContext.ExecutionProfile.ExecutionModelName);
            execution.ExecutionProfileName.Should().Be(erContext.ExecutionProfile.ProfileName);
            execution.Executor.Should().NotBeNull();
            execution.ExtensionId.Should().Be(erContext.Extension.ExtensionId);
            execution.ExtensionVersionId.Should().Be(erContext.ExtensionVersion.ExtensionVersionId);
            execution.InputObjects.Should().HaveCount(1);
            execution.InputObjects.First().Description.Should().Be(inputObject.Description);
            execution.InputObjects.First().IsRequired.Should().Be(inputObject.IsRequired);
            execution.InputObjects.First().Name.Should().Be(inputObject.Name);
            execution.Mode.Should().Be(erContext.ExecutionProfile.ExecutionMode);
            execution.ObjectProviderName.Should().Be(erContext.ExecutionProfile.ObjectProviderName);
            execution.Priority.Should().Be(Enum.Parse<ExecutionPriority>(erContext.OriginalRequest.Priority));
            execution.Status.Should().Be(ExecutionStatus.PendingInputObjects);
        }

        [Fact]
        public async Task CreateNewExecutionAsync_GivenExtensionWithNoInputObjects_InDirectMode_ShouldRespondOk()
        {
            var mockErContextBuilder = new Mock<IExecutionRequestContextBuilder>();
            var mockExecRepository = new Mock<IExecutionRepository>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();
            var mockExtensionObjectApiModelService = new Mock<IExtensionObjectApiModelService>();
            var mockExtensionRsaKeyProvider = new Mock<IExtensionRsaKeyProvider>();
            var mockExtensionSettingsBuilder = new Mock<IExtensionSettingsBuilder>();
            var mockExecRequestRouter = new Mock<IExecutionRequestRouter>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockExecUpdatePublisher = new Mock<IExecutionUpdatePublisher>();
            var mockDirectExecRequestSigner = new Mock<ISigner<DirectExecutionRequestApiModel>>();
            var userContext = CreateDefaultUserContext();

            var execController = new Controllers.ExecutionController(
                mockErContextBuilder.Object,
                mockExecRepository.Object,
                mockExtensionRepository.Object,
                mockExtensionObjectApiModelService.Object,
                mockExtensionRsaKeyProvider.Object,
                mockExtensionSettingsBuilder.Object,
                mockExecRequestRouter.Object,
                mockExecUpdatePublisher.Object,
                mockExecServiceProvider.Object,
                mockDirectExecRequestSigner.Object,
                userContext);

            var execRequestApiModel =
                new ExecutionRequestApiModel
                {
                    ExtensionId = DefaultExtensionId,
                    ExtensionVersionId = DefaultExtensionVersionId
                };

            var erContext =
                new ExecutionRequestContext<ExecutionRequestApiModel>
                {
                    OriginalRequest = execRequestApiModel,
                    Extension = CreateDefaultExtension(),
                    ExtensionVersion = CreateDefaultExtensionVersion(),
                    ExecutionProfile = CreateDefaultExecutionProfile()
                };

            erContext.ExecutionProfile.ExecutionMode = ExecutionMode.Direct;
            erContext.ExecutionProfile.DirectExecutionTokenDuration = TimeSpan.FromHours(1);

            mockExecServiceProvider
                .Setup(p => p.GetServiceConfigurationAsync(It.IsAny<ExecutionRequest>()))
                .Returns(Task.FromResult(DefaultServiceConfiguration));

            mockExtensionRsaKeyProvider
                .Setup(p => p.GetExtensionRsaKeyXmlAsync(erContext.Extension))
                .Returns(Task.FromResult(DefaultExtensionRsaKey));

            mockDirectExecRequestSigner
                .Setup(s => s.GenerateSignatureAsync(DefaultExtensionRsaKey, It.IsAny<DirectExecutionRequestApiModel>()))
                .Returns(Task.FromResult(DefaultExtensionSignature));

            mockExtensionSettingsBuilder
                .Setup(b => b.BuildExtensionSettingsAsync(erContext))
                .Returns(Task.FromResult(erContext.ExecutionProfile.ExtensionSettings));

            mockErContextBuilder
                .Setup(b => b.BuildExecutionRequestContextAsync(execRequestApiModel))
                .Returns(Task.FromResult(erContext));

            var apiResponse = await execController.CreateNewExecutionAsync(execRequestApiModel) as OkObjectResult;

            apiResponse.Should().NotBeNull();
            apiResponse.Value.Should().NotBeNull();
            apiResponse.Value.Should().BeOfType<DirectExecutionRequestApiModel>();

            var directExecApiModel = apiResponse.Value as DirectExecutionRequestApiModel;

            directExecApiModel.ExecutionModelName.Should().Be(erContext.ExecutionProfile.ExecutionModelName);
            directExecApiModel.ExecutionProfileName.Should().Be(erContext.ExecutionProfile.ProfileName);
            directExecApiModel.ExtensionId.Should().Be(erContext.Extension.ExtensionId);
            directExecApiModel.ExtensionVersionId.Should().Be(erContext.ExtensionVersion.ExtensionVersionId);
            directExecApiModel.ObjectProviderName.Should().Be(erContext.ExecutionProfile.ObjectProviderName);
            directExecApiModel.Services.Should().BeEquivalentTo(DefaultServiceConfiguration);
            directExecApiModel.Signature.Should().Be(DefaultExtensionSignature);
        }

        [Fact]
        public async Task CreateNewExecutionAsync_GivenExtensionWithNoInputObjects_InDirectMode_ShouldSaveExecution()
        {
            var mockErContextBuilder = new Mock<IExecutionRequestContextBuilder>();
            var mockExecRepository = new Mock<IExecutionRepository>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();
            var mockExtensionObjectApiModelService = new Mock<IExtensionObjectApiModelService>();
            var mockExtensionRsaKeyProvider = new Mock<IExtensionRsaKeyProvider>();
            var mockExtensionSettingsBuilder = new Mock<IExtensionSettingsBuilder>();
            var mockExecRequestRouter = new Mock<IExecutionRequestRouter>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockExecUpdatePublisher = new Mock<IExecutionUpdatePublisher>();
            var mockDirectExecRequestSigner = new Mock<ISigner<DirectExecutionRequestApiModel>>();
            var userContext = CreateDefaultUserContext();

            var execController = new Controllers.ExecutionController(
                mockErContextBuilder.Object,
                mockExecRepository.Object,
                mockExtensionRepository.Object,
                mockExtensionObjectApiModelService.Object,
                mockExtensionRsaKeyProvider.Object,
                mockExtensionSettingsBuilder.Object,
                mockExecRequestRouter.Object,
                mockExecUpdatePublisher.Object,
                mockExecServiceProvider.Object,
                mockDirectExecRequestSigner.Object,
                userContext);

            var execRequestApiModel =
                new ExecutionRequestApiModel
                {
                    ExtensionId = DefaultExtensionId,
                    ExtensionVersionId = DefaultExtensionVersionId
                };

            var erContext =
                new ExecutionRequestContext<ExecutionRequestApiModel>
                {
                    OriginalRequest = execRequestApiModel,
                    Extension = CreateDefaultExtension(),
                    ExtensionVersion = CreateDefaultExtensionVersion(),
                    ExecutionProfile = CreateDefaultExecutionProfile()
                };

            erContext.ExecutionProfile.ExecutionMode = ExecutionMode.Direct;
            erContext.ExecutionProfile.DirectExecutionTokenDuration = TimeSpan.FromHours(1);

            mockExecServiceProvider
                .Setup(p => p.GetServiceConfigurationAsync(It.IsAny<ExecutionRequest>()))
                .Returns(Task.FromResult(DefaultServiceConfiguration));

            mockExtensionRsaKeyProvider
                .Setup(p => p.GetExtensionRsaKeyXmlAsync(erContext.Extension))
                .Returns(Task.FromResult(DefaultExtensionRsaKey));

            mockDirectExecRequestSigner
                .Setup(s => s.GenerateSignatureAsync(DefaultExtensionRsaKey, It.IsAny<DirectExecutionRequestApiModel>()))
                .Returns(Task.FromResult(DefaultExtensionSignature));

            mockExtensionSettingsBuilder
                .Setup(b => b.BuildExtensionSettingsAsync(erContext))
                .Returns(Task.FromResult(erContext.ExecutionProfile.ExtensionSettings));

            mockErContextBuilder
                .Setup(b => b.BuildExecutionRequestContextAsync(execRequestApiModel))
                .Returns(Task.FromResult(erContext));

            Core.Models.Execution execution = null;

            mockExecRepository
                .Setup(r => r.UpsertExecutionAsync(It.IsAny<Core.Models.Execution>()))
                .Callback<Core.Models.Execution>(e => execution = e);

            await execController.CreateNewExecutionAsync(execRequestApiModel);

            execution.Should().NotBeNull();
            execution.ExecutionModelName.Should().Be(erContext.ExecutionProfile.ExecutionModelName);
            execution.ExecutionProfileName.Should().Be(erContext.ExecutionProfile.ProfileName);
            execution.Executor.Should().NotBeNull();
            execution.ExtensionId.Should().Be(erContext.Extension.ExtensionId);
            execution.ExtensionVersionId.Should().Be(erContext.ExtensionVersion.ExtensionVersionId);
            execution.Mode.Should().Be(erContext.ExecutionProfile.ExecutionMode);
            execution.ObjectProviderName.Should().Be(erContext.ExecutionProfile.ObjectProviderName);
            execution.Priority.Should().Be(Enum.Parse<ExecutionPriority>(erContext.OriginalRequest.Priority));
            execution.Status.Should().Be(ExecutionStatus.DirectExecutionTokenProvided);
        }

        [Fact]
        public async Task CreateNewExecutionAsync_GivenExtensionWithNoInputObjects_InDirectMode_ShouldPublishStatusUpdateEvent()
        {
            var mockErContextBuilder = new Mock<IExecutionRequestContextBuilder>();
            var mockExecRepository = new Mock<IExecutionRepository>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();
            var mockExtensionObjectApiModelService = new Mock<IExtensionObjectApiModelService>();
            var mockExtensionRsaKeyProvider = new Mock<IExtensionRsaKeyProvider>();
            var mockExtensionSettingsBuilder = new Mock<IExtensionSettingsBuilder>();
            var mockExecRequestRouter = new Mock<IExecutionRequestRouter>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockExecUpdatePublisher = new Mock<IExecutionUpdatePublisher>();
            var mockDirectExecRequestSigner = new Mock<ISigner<DirectExecutionRequestApiModel>>();
            var userContext = CreateDefaultUserContext();

            var execController = new Controllers.ExecutionController(
                mockErContextBuilder.Object,
                mockExecRepository.Object,
                mockExtensionRepository.Object,
                mockExtensionObjectApiModelService.Object,
                mockExtensionRsaKeyProvider.Object,
                mockExtensionSettingsBuilder.Object,
                mockExecRequestRouter.Object,
                mockExecUpdatePublisher.Object,
                mockExecServiceProvider.Object,
                mockDirectExecRequestSigner.Object,
                userContext);

            var execRequestApiModel =
                new ExecutionRequestApiModel
                {
                    ExtensionId = DefaultExtensionId,
                    ExtensionVersionId = DefaultExtensionVersionId
                };

            var erContext =
                new ExecutionRequestContext<ExecutionRequestApiModel>
                {
                    OriginalRequest = execRequestApiModel,
                    Extension = CreateDefaultExtension(),
                    ExtensionVersion = CreateDefaultExtensionVersion(),
                    ExecutionProfile = CreateDefaultExecutionProfile()
                };

            erContext.ExecutionProfile.ExecutionMode = ExecutionMode.Direct;
            erContext.ExecutionProfile.DirectExecutionTokenDuration = TimeSpan.FromHours(1);

            mockExecServiceProvider
                .Setup(p => p.GetServiceConfigurationAsync(It.IsAny<ExecutionRequest>()))
                .Returns(Task.FromResult(DefaultServiceConfiguration));

            mockExtensionRsaKeyProvider
                .Setup(p => p.GetExtensionRsaKeyXmlAsync(erContext.Extension))
                .Returns(Task.FromResult(DefaultExtensionRsaKey));

            mockDirectExecRequestSigner
                .Setup(s => s.GenerateSignatureAsync(DefaultExtensionRsaKey, It.IsAny<DirectExecutionRequestApiModel>()))
                .Returns(Task.FromResult(DefaultExtensionSignature));

            mockExtensionSettingsBuilder
                .Setup(b => b.BuildExtensionSettingsAsync(erContext))
                .Returns(Task.FromResult(erContext.ExecutionProfile.ExtensionSettings));

            mockErContextBuilder
                .Setup(b => b.BuildExecutionRequestContextAsync(execRequestApiModel))
                .Returns(Task.FromResult(erContext));

            Core.Models.Execution execution = null;

            mockExecUpdatePublisher
                .Setup(p => p.PublishUpdateAsync(It.IsAny<Core.Models.Execution>()))
                .Callback<Core.Models.Execution>(e => execution = e);

            await execController.CreateNewExecutionAsync(execRequestApiModel);

            execution.Should().NotBeNull();
            execution.ExecutionModelName.Should().Be(erContext.ExecutionProfile.ExecutionModelName);
            execution.ExecutionProfileName.Should().Be(erContext.ExecutionProfile.ProfileName);
            execution.Executor.Should().NotBeNull();
            execution.ExtensionId.Should().Be(erContext.Extension.ExtensionId);
            execution.ExtensionVersionId.Should().Be(erContext.ExtensionVersion.ExtensionVersionId);
            execution.Mode.Should().Be(erContext.ExecutionProfile.ExecutionMode);
            execution.ObjectProviderName.Should().Be(erContext.ExecutionProfile.ObjectProviderName);
            execution.Priority.Should().Be(Enum.Parse<ExecutionPriority>(erContext.OriginalRequest.Priority));
            execution.Status.Should().Be(ExecutionStatus.DirectExecutionTokenProvided);
        }

        [Theory]
        [InlineData(ExecutionStatus.Canceled)]
        [InlineData(ExecutionStatus.Succeeded)]
        [InlineData(ExecutionStatus.ValidationSucceeded)]
        public async Task CreateNewExecutionAsync_GivenExtensionWithNoInputObjects_InGatewayMode_ShouldRespondOk(ExecutionStatus execStatus)
        {
            var mockErContextBuilder = new Mock<IExecutionRequestContextBuilder>();
            var mockExecRepository = new Mock<IExecutionRepository>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();
            var mockExtensionObjectApiModelService = new Mock<IExtensionObjectApiModelService>();
            var mockExtensionRsaKeyProvider = new Mock<IExtensionRsaKeyProvider>();
            var mockExtensionSettingsBuilder = new Mock<IExtensionSettingsBuilder>();
            var mockExecRequestRouter = new Mock<IExecutionRequestRouter>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockExecUpdatePublisher = new Mock<IExecutionUpdatePublisher>();
            var mockDirectExecRequestSigner = new Mock<ISigner<DirectExecutionRequestApiModel>>();
            var userContext = CreateDefaultUserContext();

            var execController = new Controllers.ExecutionController(
                mockErContextBuilder.Object,
                mockExecRepository.Object,
                mockExtensionRepository.Object,
                mockExtensionObjectApiModelService.Object,
                mockExtensionRsaKeyProvider.Object,
                mockExtensionSettingsBuilder.Object,
                mockExecRequestRouter.Object,
                mockExecUpdatePublisher.Object,
                mockExecServiceProvider.Object,
                mockDirectExecRequestSigner.Object,
                userContext);

            var execRequestApiModel =
                new ExecutionRequestApiModel
                {
                    ExtensionId = DefaultExtensionId,
                    ExtensionVersionId = DefaultExtensionVersionId
                };

            var erContext =
                new ExecutionRequestContext<ExecutionRequestApiModel>
                {
                    OriginalRequest = execRequestApiModel,
                    Extension = CreateDefaultExtension(),
                    ExtensionVersion = CreateDefaultExtensionVersion(),
                    ExecutionProfile = CreateDefaultExecutionProfile()
                };

            erContext.ExecutionProfile.ExecutionMode = ExecutionMode.Gateway;

            mockExecServiceProvider
                .Setup(p => p.GetServiceConfigurationAsync(It.IsAny<ExecutionRequest>()))
                .Returns(Task.FromResult(DefaultServiceConfiguration));

            mockExtensionRsaKeyProvider
                .Setup(p => p.GetExtensionRsaKeyXmlAsync(erContext.Extension))
                .Returns(Task.FromResult(DefaultExtensionRsaKey));

            mockDirectExecRequestSigner
                .Setup(s => s.GenerateSignatureAsync(DefaultExtensionRsaKey, It.IsAny<DirectExecutionRequestApiModel>()))
                .Returns(Task.FromResult(DefaultExtensionSignature));

            mockExtensionSettingsBuilder
                .Setup(b => b.BuildExtensionSettingsAsync(erContext))
                .Returns(Task.FromResult(erContext.ExecutionProfile.ExtensionSettings));

            mockErContextBuilder
                .Setup(b => b.BuildExecutionRequestContextAsync(execRequestApiModel))
                .Returns(Task.FromResult(erContext));

            mockExecRequestRouter
                .Setup(r => r.RouteRequestAsync(It.IsAny<ExecutionRequest>(), It.IsAny<CancellationToken>()))
                .Returns((ExecutionRequest er, CancellationToken ct) => Task.FromResult(
                    new Core.Models.ExecutionContext
                    {
                        CreatedDateTimeUtc = er.CreatedDateTimeUtc,
                        ExecutionId = er.ExecutionId,
                        ExecutionModelName = er.ExecutionModelName,
                        ExecutionProfileName = er.ExecutionProfileName,
                        Executor = er.Executor,
                        ExtensionId = er.ExtensionId,
                        ExtensionVersionId = er.ExtensionVersionId,
                        LastUpdatedDateTimeUtc = DateTime.UtcNow,
                        ObjectProviderName = er.ObjectProviderName,
                        InputObjects = er.InputObjects,
                        OutputObjects = er.OutputObjects,
                        Priority = er.Priority,
                        ProvidedInputObjects = er.ProvidedInputObjects,
                        Status = execStatus,
                        StatusUpdateKey = er.StatusUpdateKey,
                        SupportedServices = er.SupportedServices
                    }));

            var apiResponse = await execController.CreateNewExecutionAsync(execRequestApiModel) as OkObjectResult;

            apiResponse.Should().NotBeNull();
            apiResponse.Value.Should().NotBeNull();
            apiResponse.Value.Should().BeOfType<ExecutionUpdateApiModel>();

            var updateApiModel = apiResponse.Value as ExecutionUpdateApiModel;

            updateApiModel.ExecutionStatus.Should().Be(execStatus.ToString());
        }

        [Theory]
        [InlineData(ExecutionStatus.Processing)]
        [InlineData(ExecutionStatus.Queued)]
        public async Task CreateNewExecutionAsync_GivenExtensionWithNoInputObjects_InGatewayMode_ShouldRespondAccepted(ExecutionStatus execStatus)
        {
            var mockErContextBuilder = new Mock<IExecutionRequestContextBuilder>();
            var mockExecRepository = new Mock<IExecutionRepository>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();
            var mockExtensionObjectApiModelService = new Mock<IExtensionObjectApiModelService>();
            var mockExtensionRsaKeyProvider = new Mock<IExtensionRsaKeyProvider>();
            var mockExtensionSettingsBuilder = new Mock<IExtensionSettingsBuilder>();
            var mockExecRequestRouter = new Mock<IExecutionRequestRouter>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockExecUpdatePublisher = new Mock<IExecutionUpdatePublisher>();
            var mockDirectExecRequestSigner = new Mock<ISigner<DirectExecutionRequestApiModel>>();
            var userContext = CreateDefaultUserContext();

            var execController = new Controllers.ExecutionController(
                mockErContextBuilder.Object,
                mockExecRepository.Object,
                mockExtensionRepository.Object,
                mockExtensionObjectApiModelService.Object,
                mockExtensionRsaKeyProvider.Object,
                mockExtensionSettingsBuilder.Object,
                mockExecRequestRouter.Object,
                mockExecUpdatePublisher.Object,
                mockExecServiceProvider.Object,
                mockDirectExecRequestSigner.Object,
                userContext);

            var execRequestApiModel =
                new ExecutionRequestApiModel
                {
                    ExtensionId = DefaultExtensionId,
                    ExtensionVersionId = DefaultExtensionVersionId
                };

            var erContext =
                new ExecutionRequestContext<ExecutionRequestApiModel>
                {
                    OriginalRequest = execRequestApiModel,
                    Extension = CreateDefaultExtension(),
                    ExtensionVersion = CreateDefaultExtensionVersion(),
                    ExecutionProfile = CreateDefaultExecutionProfile()
                };

            erContext.ExecutionProfile.ExecutionMode = ExecutionMode.Gateway;

            mockExecServiceProvider
                .Setup(p => p.GetServiceConfigurationAsync(It.IsAny<ExecutionRequest>()))
                .Returns(Task.FromResult(DefaultServiceConfiguration));

            mockExtensionRsaKeyProvider
                .Setup(p => p.GetExtensionRsaKeyXmlAsync(erContext.Extension))
                .Returns(Task.FromResult(DefaultExtensionRsaKey));

            mockDirectExecRequestSigner
                .Setup(s => s.GenerateSignatureAsync(DefaultExtensionRsaKey, It.IsAny<DirectExecutionRequestApiModel>()))
                .Returns(Task.FromResult(DefaultExtensionSignature));

            mockExtensionSettingsBuilder
                .Setup(b => b.BuildExtensionSettingsAsync(erContext))
                .Returns(Task.FromResult(erContext.ExecutionProfile.ExtensionSettings));

            mockErContextBuilder
                .Setup(b => b.BuildExecutionRequestContextAsync(execRequestApiModel))
                .Returns(Task.FromResult(erContext));

            mockExecRequestRouter
                .Setup(r => r.RouteRequestAsync(It.IsAny<ExecutionRequest>(), It.IsAny<CancellationToken>()))
                .Returns((ExecutionRequest er, CancellationToken ct) => Task.FromResult(
                    new Core.Models.ExecutionContext
                    {
                        CreatedDateTimeUtc = er.CreatedDateTimeUtc,
                        ExecutionId = er.ExecutionId,
                        ExecutionModelName = er.ExecutionModelName,
                        ExecutionProfileName = er.ExecutionProfileName,
                        Executor = er.Executor,
                        ExtensionId = er.ExtensionId,
                        ExtensionVersionId = er.ExtensionVersionId,
                        LastUpdatedDateTimeUtc = DateTime.UtcNow,
                        ObjectProviderName = er.ObjectProviderName,
                        InputObjects = er.InputObjects,
                        OutputObjects = er.OutputObjects,
                        Priority = er.Priority,
                        ProvidedInputObjects = er.ProvidedInputObjects,
                        Status = execStatus,
                        StatusUpdateKey = er.StatusUpdateKey,
                        SupportedServices = er.SupportedServices
                    }));

            var apiResponse = await execController.CreateNewExecutionAsync(execRequestApiModel) as AcceptedResult;

            apiResponse.Should().NotBeNull();
            apiResponse.Value.Should().NotBeNull();
            apiResponse.Value.Should().BeOfType<ExecutionUpdateApiModel>();

            var updateApiModel = apiResponse.Value as ExecutionUpdateApiModel;

            updateApiModel.ExecutionStatus.Should().Be(execStatus.ToString());
        }

        [Theory]
        [InlineData(ExecutionStatus.ValidationFailed)]
        public async Task CreateNewExecutionAsync_GivenExtensionWithNoInputObjects_InGatewayMode_ShouldRespondBadRequest(ExecutionStatus execStatus)
        {
            var mockErContextBuilder = new Mock<IExecutionRequestContextBuilder>();
            var mockExecRepository = new Mock<IExecutionRepository>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();
            var mockExtensionObjectApiModelService = new Mock<IExtensionObjectApiModelService>();
            var mockExtensionRsaKeyProvider = new Mock<IExtensionRsaKeyProvider>();
            var mockExtensionSettingsBuilder = new Mock<IExtensionSettingsBuilder>();
            var mockExecRequestRouter = new Mock<IExecutionRequestRouter>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockExecUpdatePublisher = new Mock<IExecutionUpdatePublisher>();
            var mockDirectExecRequestSigner = new Mock<ISigner<DirectExecutionRequestApiModel>>();
            var userContext = CreateDefaultUserContext();

            var execController = new Controllers.ExecutionController(
                mockErContextBuilder.Object,
                mockExecRepository.Object,
                mockExtensionRepository.Object,
                mockExtensionObjectApiModelService.Object,
                mockExtensionRsaKeyProvider.Object,
                mockExtensionSettingsBuilder.Object,
                mockExecRequestRouter.Object,
                mockExecUpdatePublisher.Object,
                mockExecServiceProvider.Object,
                mockDirectExecRequestSigner.Object,
                userContext);

            var execRequestApiModel =
                new ExecutionRequestApiModel
                {
                    ExtensionId = DefaultExtensionId,
                    ExtensionVersionId = DefaultExtensionVersionId
                };

            var erContext =
                new ExecutionRequestContext<ExecutionRequestApiModel>
                {
                    OriginalRequest = execRequestApiModel,
                    Extension = CreateDefaultExtension(),
                    ExtensionVersion = CreateDefaultExtensionVersion(),
                    ExecutionProfile = CreateDefaultExecutionProfile()
                };

            erContext.ExecutionProfile.ExecutionMode = ExecutionMode.Gateway;

            mockExecServiceProvider
                .Setup(p => p.GetServiceConfigurationAsync(It.IsAny<ExecutionRequest>()))
                .Returns(Task.FromResult(DefaultServiceConfiguration));

            mockExtensionRsaKeyProvider
                .Setup(p => p.GetExtensionRsaKeyXmlAsync(erContext.Extension))
                .Returns(Task.FromResult(DefaultExtensionRsaKey));

            mockDirectExecRequestSigner
                .Setup(s => s.GenerateSignatureAsync(DefaultExtensionRsaKey, It.IsAny<DirectExecutionRequestApiModel>()))
                .Returns(Task.FromResult(DefaultExtensionSignature));

            mockExtensionSettingsBuilder
                .Setup(b => b.BuildExtensionSettingsAsync(erContext))
                .Returns(Task.FromResult(erContext.ExecutionProfile.ExtensionSettings));

            mockErContextBuilder
                .Setup(b => b.BuildExecutionRequestContextAsync(execRequestApiModel))
                .Returns(Task.FromResult(erContext));

            mockExecRequestRouter
                .Setup(r => r.RouteRequestAsync(It.IsAny<ExecutionRequest>(), It.IsAny<CancellationToken>()))
                .Returns((ExecutionRequest er, CancellationToken ct) => Task.FromResult(
                    new Core.Models.ExecutionContext
                    {
                        CreatedDateTimeUtc = er.CreatedDateTimeUtc,
                        ExecutionId = er.ExecutionId,
                        ExecutionModelName = er.ExecutionModelName,
                        ExecutionProfileName = er.ExecutionProfileName,
                        Executor = er.Executor,
                        ExtensionId = er.ExtensionId,
                        ExtensionVersionId = er.ExtensionVersionId,
                        LastUpdatedDateTimeUtc = DateTime.UtcNow,
                        ObjectProviderName = er.ObjectProviderName,
                        InputObjects = er.InputObjects,
                        OutputObjects = er.OutputObjects,
                        Priority = er.Priority,
                        ProvidedInputObjects = er.ProvidedInputObjects,
                        Status = execStatus,
                        StatusUpdateKey = er.StatusUpdateKey,
                        SupportedServices = er.SupportedServices
                    }));

            var apiResponse = await execController.CreateNewExecutionAsync(execRequestApiModel) as BadRequestObjectResult;

            apiResponse.Should().NotBeNull();
            apiResponse.Value.Should().NotBeNull();
            apiResponse.Value.Should().BeOfType<ExecutionUpdateApiModel>();

            var updateApiModel = apiResponse.Value as ExecutionUpdateApiModel;

            updateApiModel.ExecutionStatus.Should().Be(execStatus.ToString());
        }

        [Theory]
        [InlineData(ExecutionStatus.DirectExecutionTokenProvided)]
        [InlineData(ExecutionStatus.Failed)]
        [InlineData(ExecutionStatus.TimedOut)]
        [InlineData(ExecutionStatus.Undefined)]
        public async Task CreateNewExecutionAsync_GivenExtensionWithNoInputObjects_InGatewayMode_ShouldRespondError(ExecutionStatus execStatus)
        {
            var mockErContextBuilder = new Mock<IExecutionRequestContextBuilder>();
            var mockExecRepository = new Mock<IExecutionRepository>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();
            var mockExtensionObjectApiModelService = new Mock<IExtensionObjectApiModelService>();
            var mockExtensionRsaKeyProvider = new Mock<IExtensionRsaKeyProvider>();
            var mockExtensionSettingsBuilder = new Mock<IExtensionSettingsBuilder>();
            var mockExecRequestRouter = new Mock<IExecutionRequestRouter>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockExecUpdatePublisher = new Mock<IExecutionUpdatePublisher>();
            var mockDirectExecRequestSigner = new Mock<ISigner<DirectExecutionRequestApiModel>>();
            var userContext = CreateDefaultUserContext();

            var execController = new Controllers.ExecutionController(
                mockErContextBuilder.Object,
                mockExecRepository.Object,
                mockExtensionRepository.Object,
                mockExtensionObjectApiModelService.Object,
                mockExtensionRsaKeyProvider.Object,
                mockExtensionSettingsBuilder.Object,
                mockExecRequestRouter.Object,
                mockExecUpdatePublisher.Object,
                mockExecServiceProvider.Object,
                mockDirectExecRequestSigner.Object,
                userContext);

            var execRequestApiModel =
                new ExecutionRequestApiModel
                {
                    ExtensionId = DefaultExtensionId,
                    ExtensionVersionId = DefaultExtensionVersionId
                };

            var erContext =
                new ExecutionRequestContext<ExecutionRequestApiModel>
                {
                    OriginalRequest = execRequestApiModel,
                    Extension = CreateDefaultExtension(),
                    ExtensionVersion = CreateDefaultExtensionVersion(),
                    ExecutionProfile = CreateDefaultExecutionProfile()
                };

            erContext.ExecutionProfile.ExecutionMode = ExecutionMode.Gateway;

            mockExecServiceProvider
                .Setup(p => p.GetServiceConfigurationAsync(It.IsAny<ExecutionRequest>()))
                .Returns(Task.FromResult(DefaultServiceConfiguration));

            mockExtensionRsaKeyProvider
                .Setup(p => p.GetExtensionRsaKeyXmlAsync(erContext.Extension))
                .Returns(Task.FromResult(DefaultExtensionRsaKey));

            mockDirectExecRequestSigner
                .Setup(s => s.GenerateSignatureAsync(DefaultExtensionRsaKey, It.IsAny<DirectExecutionRequestApiModel>()))
                .Returns(Task.FromResult(DefaultExtensionSignature));

            mockExtensionSettingsBuilder
                .Setup(b => b.BuildExtensionSettingsAsync(erContext))
                .Returns(Task.FromResult(erContext.ExecutionProfile.ExtensionSettings));

            mockErContextBuilder
                .Setup(b => b.BuildExecutionRequestContextAsync(execRequestApiModel))
                .Returns(Task.FromResult(erContext));

            mockExecRequestRouter
                .Setup(r => r.RouteRequestAsync(It.IsAny<ExecutionRequest>(), It.IsAny<CancellationToken>()))
                .Returns((ExecutionRequest er, CancellationToken ct) => Task.FromResult(
                    new Core.Models.ExecutionContext
                    {
                        CreatedDateTimeUtc = er.CreatedDateTimeUtc,
                        ExecutionId = er.ExecutionId,
                        ExecutionModelName = er.ExecutionModelName,
                        ExecutionProfileName = er.ExecutionProfileName,
                        Executor = er.Executor,
                        ExtensionId = er.ExtensionId,
                        ExtensionVersionId = er.ExtensionVersionId,
                        LastUpdatedDateTimeUtc = DateTime.UtcNow,
                        ObjectProviderName = er.ObjectProviderName,
                        InputObjects = er.InputObjects,
                        OutputObjects = er.OutputObjects,
                        Priority = er.Priority,
                        ProvidedInputObjects = er.ProvidedInputObjects,
                        Status = execStatus,
                        StatusUpdateKey = er.StatusUpdateKey,
                        SupportedServices = er.SupportedServices
                    }));

            var apiResponse = await execController.CreateNewExecutionAsync(execRequestApiModel) as ObjectResult;

            apiResponse.Should().NotBeNull();
            apiResponse.StatusCode.Should().Be((int)(HttpStatusCode.InternalServerError));
            apiResponse.Value.Should().NotBeNull();
            apiResponse.Value.Should().BeOfType<ExecutionUpdateApiModel>();

            var updateApiModel = apiResponse.Value as ExecutionUpdateApiModel;

            updateApiModel.ExecutionStatus.Should().Be(execStatus.ToString());
        }

        [Theory]
        [InlineData(ExecutionStatus.Canceled)]
        [InlineData(ExecutionStatus.Failed)]
        [InlineData(ExecutionStatus.Processing)]
        [InlineData(ExecutionStatus.Queued)]
        [InlineData(ExecutionStatus.Succeeded)]
        [InlineData(ExecutionStatus.TimedOut)]
        [InlineData(ExecutionStatus.ValidationFailed)]
        [InlineData(ExecutionStatus.ValidationSucceeded)]
        public async Task CreateNewExecutionAsync_GivenExtensionWithNoInputObjects_InGatewayMode_ShouldSaveExecution(ExecutionStatus execStatus)
        {
            var mockErContextBuilder = new Mock<IExecutionRequestContextBuilder>();
            var mockExecRepository = new Mock<IExecutionRepository>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();
            var mockExtensionObjectApiModelService = new Mock<IExtensionObjectApiModelService>();
            var mockExtensionRsaKeyProvider = new Mock<IExtensionRsaKeyProvider>();
            var mockExtensionSettingsBuilder = new Mock<IExtensionSettingsBuilder>();
            var mockExecRequestRouter = new Mock<IExecutionRequestRouter>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockExecUpdatePublisher = new Mock<IExecutionUpdatePublisher>();
            var mockDirectExecRequestSigner = new Mock<ISigner<DirectExecutionRequestApiModel>>();
            var userContext = CreateDefaultUserContext();

            var execController = new Controllers.ExecutionController(
                mockErContextBuilder.Object,
                mockExecRepository.Object,
                mockExtensionRepository.Object,
                mockExtensionObjectApiModelService.Object,
                mockExtensionRsaKeyProvider.Object,
                mockExtensionSettingsBuilder.Object,
                mockExecRequestRouter.Object,
                mockExecUpdatePublisher.Object,
                mockExecServiceProvider.Object,
                mockDirectExecRequestSigner.Object,
                userContext);

            var execRequestApiModel =
                new ExecutionRequestApiModel
                {
                    ExtensionId = DefaultExtensionId,
                    ExtensionVersionId = DefaultExtensionVersionId
                };

            var erContext =
                new ExecutionRequestContext<ExecutionRequestApiModel>
                {
                    OriginalRequest = execRequestApiModel,
                    Extension = CreateDefaultExtension(),
                    ExtensionVersion = CreateDefaultExtensionVersion(),
                    ExecutionProfile = CreateDefaultExecutionProfile()
                };

            erContext.ExecutionProfile.ExecutionMode = ExecutionMode.Gateway;

            mockExecServiceProvider
                .Setup(p => p.GetServiceConfigurationAsync(It.IsAny<ExecutionRequest>()))
                .Returns(Task.FromResult(DefaultServiceConfiguration));

            mockExtensionRsaKeyProvider
                .Setup(p => p.GetExtensionRsaKeyXmlAsync(erContext.Extension))
                .Returns(Task.FromResult(DefaultExtensionRsaKey));

            mockDirectExecRequestSigner
                .Setup(s => s.GenerateSignatureAsync(DefaultExtensionRsaKey, It.IsAny<DirectExecutionRequestApiModel>()))
                .Returns(Task.FromResult(DefaultExtensionSignature));

            mockExtensionSettingsBuilder
                .Setup(b => b.BuildExtensionSettingsAsync(erContext))
                .Returns(Task.FromResult(erContext.ExecutionProfile.ExtensionSettings));

            mockErContextBuilder
                .Setup(b => b.BuildExecutionRequestContextAsync(execRequestApiModel))
                .Returns(Task.FromResult(erContext));

            mockExecRequestRouter
                .Setup(r => r.RouteRequestAsync(It.IsAny<ExecutionRequest>(), It.IsAny<CancellationToken>()))
                .Returns((ExecutionRequest er, CancellationToken ct) => Task.FromResult(
                    new Core.Models.ExecutionContext
                    {
                        CreatedDateTimeUtc = er.CreatedDateTimeUtc,
                        ExecutionId = er.ExecutionId,
                        ExecutionModelName = er.ExecutionModelName,
                        ExecutionProfileName = er.ExecutionProfileName,
                        Executor = er.Executor,
                        ExtensionId = er.ExtensionId,
                        ExtensionVersionId = er.ExtensionVersionId,
                        LastUpdatedDateTimeUtc = DateTime.UtcNow,
                        ObjectProviderName = er.ObjectProviderName,
                        InputObjects = er.InputObjects,
                        OutputObjects = er.OutputObjects,
                        Priority = er.Priority,
                        ProvidedInputObjects = er.ProvidedInputObjects,
                        Status = execStatus,
                        StatusUpdateKey = er.StatusUpdateKey,
                        SupportedServices = er.SupportedServices
                    }));

            Core.Models.Execution execution = null;

            mockExecRepository
                .Setup(r => r.UpsertExecutionAsync(It.IsAny<Core.Models.Execution>()))
                .Callback<Core.Models.Execution>(e => execution = e);

            await execController.CreateNewExecutionAsync(execRequestApiModel);

            execution.Should().NotBeNull();
            execution.ExecutionModelName.Should().Be(erContext.ExecutionProfile.ExecutionModelName);
            execution.ExecutionProfileName.Should().Be(erContext.ExecutionProfile.ProfileName);
            execution.Executor.Should().NotBeNull();
            execution.ExtensionId.Should().Be(erContext.Extension.ExtensionId);
            execution.ExtensionVersionId.Should().Be(erContext.ExtensionVersion.ExtensionVersionId);
            execution.Mode.Should().Be(erContext.ExecutionProfile.ExecutionMode);
            execution.ObjectProviderName.Should().Be(erContext.ExecutionProfile.ObjectProviderName);
            execution.Priority.Should().Be(Enum.Parse<ExecutionPriority>(erContext.OriginalRequest.Priority));
            execution.Status.Should().Be(execStatus);
        }

        [Theory]
        [InlineData(ExecutionStatus.Canceled)]
        [InlineData(ExecutionStatus.Failed)]
        [InlineData(ExecutionStatus.Processing)]
        [InlineData(ExecutionStatus.Queued)]
        [InlineData(ExecutionStatus.Succeeded)]
        [InlineData(ExecutionStatus.TimedOut)]
        [InlineData(ExecutionStatus.ValidationFailed)]
        [InlineData(ExecutionStatus.ValidationSucceeded)]
        public async Task CreateNewExecutionAsync_GivenExtensionWithNoInputObjects_InGatewayMode_ShouldPublishStatusUpdateEvent(ExecutionStatus execStatus)
        {
            var mockErContextBuilder = new Mock<IExecutionRequestContextBuilder>();
            var mockExecRepository = new Mock<IExecutionRepository>();
            var mockExtensionRepository = new Mock<IExtensionRepository>();
            var mockExtensionObjectApiModelService = new Mock<IExtensionObjectApiModelService>();
            var mockExtensionRsaKeyProvider = new Mock<IExtensionRsaKeyProvider>();
            var mockExtensionSettingsBuilder = new Mock<IExtensionSettingsBuilder>();
            var mockExecRequestRouter = new Mock<IExecutionRequestRouter>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockExecUpdatePublisher = new Mock<IExecutionUpdatePublisher>();
            var mockDirectExecRequestSigner = new Mock<ISigner<DirectExecutionRequestApiModel>>();
            var userContext = CreateDefaultUserContext();

            var execController = new Controllers.ExecutionController(
                mockErContextBuilder.Object,
                mockExecRepository.Object,
                mockExtensionRepository.Object,
                mockExtensionObjectApiModelService.Object,
                mockExtensionRsaKeyProvider.Object,
                mockExtensionSettingsBuilder.Object,
                mockExecRequestRouter.Object,
                mockExecUpdatePublisher.Object,
                mockExecServiceProvider.Object,
                mockDirectExecRequestSigner.Object,
                userContext);

            var execRequestApiModel =
                new ExecutionRequestApiModel
                {
                    ExtensionId = DefaultExtensionId,
                    ExtensionVersionId = DefaultExtensionVersionId
                };

            var erContext =
                new ExecutionRequestContext<ExecutionRequestApiModel>
                {
                    OriginalRequest = execRequestApiModel,
                    Extension = CreateDefaultExtension(),
                    ExtensionVersion = CreateDefaultExtensionVersion(),
                    ExecutionProfile = CreateDefaultExecutionProfile()
                };

            erContext.ExecutionProfile.ExecutionMode = ExecutionMode.Gateway;

            mockExecServiceProvider
                .Setup(p => p.GetServiceConfigurationAsync(It.IsAny<ExecutionRequest>()))
                .Returns(Task.FromResult(DefaultServiceConfiguration));

            mockExtensionRsaKeyProvider
                .Setup(p => p.GetExtensionRsaKeyXmlAsync(erContext.Extension))
                .Returns(Task.FromResult(DefaultExtensionRsaKey));

            mockDirectExecRequestSigner
                .Setup(s => s.GenerateSignatureAsync(DefaultExtensionRsaKey, It.IsAny<DirectExecutionRequestApiModel>()))
                .Returns(Task.FromResult(DefaultExtensionSignature));

            mockExtensionSettingsBuilder
                .Setup(b => b.BuildExtensionSettingsAsync(erContext))
                .Returns(Task.FromResult(erContext.ExecutionProfile.ExtensionSettings));

            mockErContextBuilder
                .Setup(b => b.BuildExecutionRequestContextAsync(execRequestApiModel))
                .Returns(Task.FromResult(erContext));

            mockExecRequestRouter
                .Setup(r => r.RouteRequestAsync(It.IsAny<ExecutionRequest>(), It.IsAny<CancellationToken>()))
                .Returns((ExecutionRequest er, CancellationToken ct) => Task.FromResult(
                    new Core.Models.ExecutionContext
                    {
                        CreatedDateTimeUtc = er.CreatedDateTimeUtc,
                        ExecutionId = er.ExecutionId,
                        ExecutionModelName = er.ExecutionModelName,
                        ExecutionProfileName = er.ExecutionProfileName,
                        Executor = er.Executor,
                        ExtensionId = er.ExtensionId,
                        ExtensionVersionId = er.ExtensionVersionId,
                        LastUpdatedDateTimeUtc = DateTime.UtcNow,
                        ObjectProviderName = er.ObjectProviderName,
                        InputObjects = er.InputObjects,
                        OutputObjects = er.OutputObjects,
                        Priority = er.Priority,
                        ProvidedInputObjects = er.ProvidedInputObjects,
                        Status = execStatus,
                        StatusUpdateKey = er.StatusUpdateKey,
                        SupportedServices = er.SupportedServices
                    }));

            Core.Models.Execution execution = null;

            mockExecUpdatePublisher
                .Setup(p => p.PublishUpdateAsync(It.IsAny<Core.Models.Execution>()))
                .Callback<Core.Models.Execution>(e => execution = e);

            await execController.CreateNewExecutionAsync(execRequestApiModel);

            execution.Should().NotBeNull();
            execution.ExecutionModelName.Should().Be(erContext.ExecutionProfile.ExecutionModelName);
            execution.ExecutionProfileName.Should().Be(erContext.ExecutionProfile.ProfileName);
            execution.Executor.Should().NotBeNull();
            execution.ExtensionId.Should().Be(erContext.Extension.ExtensionId);
            execution.ExtensionVersionId.Should().Be(erContext.ExtensionVersion.ExtensionVersionId);
            execution.Mode.Should().Be(erContext.ExecutionProfile.ExecutionMode);
            execution.ObjectProviderName.Should().Be(erContext.ExecutionProfile.ObjectProviderName);
            execution.Priority.Should().Be(Enum.Parse<ExecutionPriority>(erContext.OriginalRequest.Priority));
            execution.Status.Should().Be(execStatus);
        }

        private ExtensionInputObject CreateDefaultExtensionInputObject() =>
            new ExtensionInputObject
            {
                Name = "Object 1",
                Description = "This is object 1.",
                IsRequired = true
            };

        private InputObjectApiModel CreateDefaultInputObjectApiModel() =>
            new InputObjectApiModel
            {
                Accessor = CreateDefaultObjectAccessorJson(),
                Description = "This is object 1.",
                IsRequired = true,
                ObjectProviderName = DefaultObjectProviderName
            };

        private JObject CreateDefaultObjectAccessorJson() =>
            JObject.FromObject(new ObjectUrl
            {
                Url = "http://some.url.com",
                HttpMethod = HttpMethod.Get.ToString()
            });

        private IUserContext CreateDefaultUserContext() =>
            new UserContext
            {
                Executor = CreateDefaultExecutorContext()
            };

        private ExecutorContext CreateDefaultExecutorContext() =>
            new ExecutorContext
            {
                TenantId = DefaultTenantId,
                UserId = DefaultUserId
            };

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
