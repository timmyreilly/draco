// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Adapters;
using Draco.Core.Execution.Models;
using Draco.Core.Interfaces;
using Draco.Core.Models;
using Draco.Core.Models.Enumerations;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.ObjectStorage.Models;
using Draco.Core.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Draco.Core.Execution.UnitTests
{
    public class JsonHttpExecutionAdapterTests
    {
        const string DefaultExecutionModelName = "DefaultExecutionModel";
        const string DefaultExecutionProfileName = "DefaultExecutionProfileName";
        const string DefaultObjectProviderName = "DefaultObjectProvider";
        const string DefaultGetExecutionStatusUrl = "http://test.com/execution/status";
        const string DefaultUpdateExecutionStatusUrl = "http://test.com/execution/status";
        const string DefaultExtensionExecutionUrl = "http://test.com/extension/execute";
        const string DefaultExtensionValidationUrl = "http://test.com/extension/validate";

        readonly string[] inputObjectNames = { "InputObjectA", "InputObjectB" };
        readonly string[] outputObjectNames = { "OutputObjectA", "OutputObjectB" };
        readonly HttpExtensionSettings defaultHttpExtensionSettings;
        readonly JObject defaultExecutionServiceConfiguration;
        readonly Dictionary<string, ExtensionInputObject> defaultInputObjectDictionary;
        readonly Dictionary<string, ExtensionOutputObject> defaultOutputObjectDictionary;
        readonly Dictionary<string, JObject> defaultInputObjectAccessorDictionary;
        readonly Dictionary<string, JObject> defaultOutputObjectAccessorDictionary;

        public JsonHttpExecutionAdapterTests()
        {
            defaultHttpExtensionSettings = CreateDefaultHttpExtensionSettings();
            defaultExecutionServiceConfiguration = CreateDefaultExecutionServiceConfiguration();
            defaultInputObjectDictionary = CreateDefaultInputObjectDictionary();
            defaultOutputObjectDictionary = CreateDefaultOutputObjectDictionary();
            defaultInputObjectAccessorDictionary = CreateDefaultInputObjectAccessorDictionary();
            defaultOutputObjectAccessorDictionary = CreateDefaultOutputObjectAccessorDictionary();
        }

        [Fact]
        public async void ExecuteAsync_NullExecutionRequest_ShouldThrowException()
        {
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockInputAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var mockOutputAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var mockSigner = new Mock<ISigner<HttpExecutionRequest>>();
            var mockLogger = new Mock<ILogger<JsonHttpExecutionAdapter>>();

            var execAdapter = new JsonHttpExecutionAdapter(
                mockLogger.Object,
                mockJsonHttpClient.Object,
                mockExecServiceProvider.Object,
                mockInputAccessorProvider.Object,
                mockOutputAccessorProvider.Object,
                mockSigner.Object);

            Func<Task> act = async () => await execAdapter.ExecuteAsync(null, CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async void ExecuteAsync_ValidateOnlyButExtensionDoesntSupportValidation_ShouldThrowException()
        {
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockInputAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var mockOutputAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var mockSigner = new Mock<ISigner<HttpExecutionRequest>>();
            var mockLogger = new Mock<ILogger<JsonHttpExecutionAdapter>>();

            var execAdapter = new JsonHttpExecutionAdapter(
                mockLogger.Object,
                mockJsonHttpClient.Object,
                mockExecServiceProvider.Object,
                mockInputAccessorProvider.Object,
                mockOutputAccessorProvider.Object,
                mockSigner.Object);

            var execRequest = CreateDefaultExecutionRequest();

            execRequest.ValidateOnly = true;
            execRequest.IsValidationSupported = false;

            SetupDefaultMockExecutionServiceProvider(execRequest, mockExecServiceProvider);
            SetupDefaultMockInputObjectAccessorProvider(execRequest, mockInputAccessorProvider);
            SetupDefaultMockOutputObjectAccessorProvider(execRequest, mockOutputAccessorProvider);

            var execContext = await execAdapter.ExecuteAsync(execRequest, CancellationToken.None);

            execContext.Status.Should().Be(ExecutionStatus.Failed);
        }

        [Fact]
        public async void ExecuteAsync_ValidationRequested_ShouldPostValidationRequestToExtension()
        {
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockInputAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var mockOutputAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var mockSigner = new Mock<ISigner<HttpExecutionRequest>>();
            var mockLogger = new Mock<ILogger<JsonHttpExecutionAdapter>>();

            var execAdapter = new JsonHttpExecutionAdapter(
                mockLogger.Object,
                mockJsonHttpClient.Object,
                mockExecServiceProvider.Object,
                mockInputAccessorProvider.Object,
                mockOutputAccessorProvider.Object,
                mockSigner.Object);

            var execRequest = CreateDefaultExecutionRequest();

            execRequest.ValidateOnly = true;
            execRequest.IsValidationSupported = true;

            SetupDefaultMockExecutionServiceProvider(execRequest, mockExecServiceProvider);
            SetupDefaultMockInputObjectAccessorProvider(execRequest, mockInputAccessorProvider);
            SetupDefaultMockOutputObjectAccessorProvider(execRequest, mockOutputAccessorProvider);

            string validationUrl = null;
            HttpExecutionRequest httpExecRequest = null;

            execRequest.ExecutionParameters = JObject.FromObject(execRequest);

            var httpExecResponse = new HttpExecutionResponse { ExecutionId = execRequest.ExecutionId };

            mockJsonHttpClient
                .Setup(hc => hc.PostAsync<HttpExecutionResponse>(defaultHttpExtensionSettings.ValidationUrl, It.IsAny<HttpExecutionRequest>()))
                .Returns(Task.FromResult(new HttpResponse<HttpExecutionResponse>(HttpStatusCode.OK, httpExecResponse)))
                .Callback<string, object>((u, er) =>
                {
                    validationUrl = u;
                    httpExecRequest = er as HttpExecutionRequest;
                });

            await execAdapter.ExecuteAsync(execRequest, CancellationToken.None);

            validationUrl.Should().NotBeNull();
            httpExecRequest.Should().NotBeNull();

            validationUrl.Should().Be(DefaultExtensionValidationUrl);

            httpExecRequest.CreatedDateTimeUtc.Should().Be(execRequest.CreatedDateTimeUtc);
            httpExecRequest.ExecutionId.Should().Be(execRequest.ExecutionId);
            httpExecRequest.ExecutionProfileName.Should().Be(execRequest.ExecutionProfileName);
            httpExecRequest.Executor.Should().Be(execRequest.Executor);
            httpExecRequest.ExpirationDateTimeUtc.Should().Be(execRequest.ExecutionTimeoutDateTimeUtc);
            httpExecRequest.ExtensionId.Should().Be(execRequest.ExtensionId);
            httpExecRequest.ExtensionVersionId.Should().Be(execRequest.ExtensionVersionId);
            httpExecRequest.GetExecutionStatusUrl.Should().Be(execRequest.GetExecutionStatusUrl);
            httpExecRequest.Priority.Should().Be(execRequest.Priority);
            httpExecRequest.Services.Should().BeEquivalentTo(defaultExecutionServiceConfiguration);
            httpExecRequest.StatusUpdateKey.Should().Be(execRequest.StatusUpdateKey);
            httpExecRequest.UpdateExecutionStatusUrl.Should().Be(execRequest.UpdateExecutionStatusUrl);
            httpExecRequest.RequestParameters.Should().BeEquivalentTo(execRequest.ExecutionParameters);
        }

        [Fact]
        public async Task ExecuteAsync_ValidationRequested_StatusShouldBeValidationPassedIfOk()
        {
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockInputAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var mockOutputAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var mockSigner = new Mock<ISigner<HttpExecutionRequest>>();
            var mockLogger = new Mock<ILogger<JsonHttpExecutionAdapter>>();

            var execAdapter = new JsonHttpExecutionAdapter(
                mockLogger.Object,
                mockJsonHttpClient.Object,
                mockExecServiceProvider.Object,
                mockInputAccessorProvider.Object,
                mockOutputAccessorProvider.Object,
                mockSigner.Object);

            var execRequest = CreateDefaultExecutionRequest();

            execRequest.ValidateOnly = true;
            execRequest.IsValidationSupported = true;

            SetupDefaultMockExecutionServiceProvider(execRequest, mockExecServiceProvider);
            SetupDefaultMockInputObjectAccessorProvider(execRequest, mockInputAccessorProvider);
            SetupDefaultMockOutputObjectAccessorProvider(execRequest, mockOutputAccessorProvider);

            var httpExecResponse = new HttpExecutionResponse { ExecutionId = execRequest.ExecutionId };

            mockJsonHttpClient
               .Setup(hc => hc.PostAsync<HttpExecutionResponse>(defaultHttpExtensionSettings.ValidationUrl, It.IsAny<HttpExecutionRequest>()))
               .Returns(Task.FromResult(new HttpResponse<HttpExecutionResponse>(HttpStatusCode.OK, httpExecResponse)));

            var execContext = await execAdapter.ExecuteAsync(execRequest, CancellationToken.None);

            execContext.Status.Should().Be(ExecutionStatus.ValidationSucceeded);
        }

        [Fact]
        public async Task ExecuteAsync_ValidationRequested_StatusShouldBeValidationFailedIfBadRequestAndErrorsProvided()
        {
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockInputAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var mockOutputAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var mockSigner = new Mock<ISigner<HttpExecutionRequest>>();
            var mockLogger = new Mock<ILogger<JsonHttpExecutionAdapter>>();

            var execAdapter = new JsonHttpExecutionAdapter(
                mockLogger.Object,
                mockJsonHttpClient.Object,
                mockExecServiceProvider.Object,
                mockInputAccessorProvider.Object,
                mockOutputAccessorProvider.Object,
                mockSigner.Object);

            var execRequest = CreateDefaultExecutionRequest();

            execRequest.ValidateOnly = true;
            execRequest.IsValidationSupported = true;

            SetupDefaultMockExecutionServiceProvider(execRequest, mockExecServiceProvider);
            SetupDefaultMockInputObjectAccessorProvider(execRequest, mockInputAccessorProvider);
            SetupDefaultMockOutputObjectAccessorProvider(execRequest, mockOutputAccessorProvider);

            var httpValidationError = new HttpExecutionValidationError
            {
                ErrorCode = "Wrong",
                ErrorId = CreateRandomId(),
                ErrorMessage = "You did it wrong."
            };

            var httpExecResponse = new HttpExecutionResponse
            {
                ExecutionId = execRequest.ExecutionId,
                ValidationErrors = new List<HttpExecutionValidationError> { httpValidationError }
            };

            mockJsonHttpClient
               .Setup(hc => hc.PostAsync<HttpExecutionResponse>(defaultHttpExtensionSettings.ValidationUrl, It.IsAny<HttpExecutionRequest>()))
               .Returns(Task.FromResult(new HttpResponse<HttpExecutionResponse>(HttpStatusCode.BadRequest, httpExecResponse)));

            var execContext = await execAdapter.ExecuteAsync(execRequest, CancellationToken.None);

            execContext.Status.Should().Be(ExecutionStatus.ValidationFailed);
            execContext.ValidationErrors.Should().HaveCount(1);

            var validationError = execContext.ValidationErrors[0];

            validationError.ErrorCode.Should().Be(httpValidationError.ErrorCode);
            validationError.ErrorId.Should().Be(httpValidationError.ErrorId);
            validationError.ErrorMessage.Should().Be(httpValidationError.ErrorMessage);
        }

        [Fact]
        public async Task ExecuteAsync_ValidationRequested_StatusShouldBeFailedIfBadRequestAndNoErrorsProvided()
        {
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockInputAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var mockOutputAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var mockSigner = new Mock<ISigner<HttpExecutionRequest>>();
            var mockLogger = new Mock<ILogger<JsonHttpExecutionAdapter>>();

            var execAdapter = new JsonHttpExecutionAdapter(
                mockLogger.Object,
                mockJsonHttpClient.Object,
                mockExecServiceProvider.Object,
                mockInputAccessorProvider.Object,
                mockOutputAccessorProvider.Object,
                mockSigner.Object);

            var execRequest = CreateDefaultExecutionRequest();

            execRequest.ValidateOnly = true;
            execRequest.IsValidationSupported = true;

            SetupDefaultMockExecutionServiceProvider(execRequest, mockExecServiceProvider);
            SetupDefaultMockInputObjectAccessorProvider(execRequest, mockInputAccessorProvider);
            SetupDefaultMockOutputObjectAccessorProvider(execRequest, mockOutputAccessorProvider);

            var httpExecResponse = new HttpExecutionResponse { ExecutionId = execRequest.ExecutionId };

            mockJsonHttpClient
               .Setup(hc => hc.PostAsync<HttpExecutionResponse>(defaultHttpExtensionSettings.ValidationUrl, It.IsAny<HttpExecutionRequest>()))
               .Returns(Task.FromResult(new HttpResponse<HttpExecutionResponse>(HttpStatusCode.BadRequest, httpExecResponse)));

            var execContext = await execAdapter.ExecuteAsync(execRequest, CancellationToken.None);

            execContext.Status.Should().Be(ExecutionStatus.Failed);
            execContext.ValidationErrors.Should().BeEmpty();
        }

        [Fact]
        public async Task ExecuteAsync_ValidationRequested_StatusShouldBeFailedIfOtherStatusCode()
        {
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockInputAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var mockOutputAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var mockSigner = new Mock<ISigner<HttpExecutionRequest>>();
            var mockLogger = new Mock<ILogger<JsonHttpExecutionAdapter>>();

            var execAdapter = new JsonHttpExecutionAdapter(
                mockLogger.Object,
                mockJsonHttpClient.Object,
                mockExecServiceProvider.Object,
                mockInputAccessorProvider.Object,
                mockOutputAccessorProvider.Object,
                mockSigner.Object);

            var execRequest = CreateDefaultExecutionRequest();

            execRequest.ValidateOnly = true;
            execRequest.IsValidationSupported = true;

            SetupDefaultMockExecutionServiceProvider(execRequest, mockExecServiceProvider);
            SetupDefaultMockInputObjectAccessorProvider(execRequest, mockInputAccessorProvider);
            SetupDefaultMockOutputObjectAccessorProvider(execRequest, mockOutputAccessorProvider);

            var httpExecResponse = new HttpExecutionResponse { ExecutionId = execRequest.ExecutionId };

            mockJsonHttpClient
               .Setup(hc => hc.PostAsync<HttpExecutionResponse>(defaultHttpExtensionSettings.ValidationUrl, It.IsAny<HttpExecutionRequest>()))
               .Returns(Task.FromResult(new HttpResponse<HttpExecutionResponse>(HttpStatusCode.InternalServerError, httpExecResponse)));

            var execContext = await execAdapter.ExecuteAsync(execRequest, CancellationToken.None);

            execContext.Status.Should().Be(ExecutionStatus.Failed);
            execContext.ValidationErrors.Should().BeEmpty();
        }

        [Fact]
        public async Task ExecuteAsync_ValidationRequested_ShouldInvokeOnValidating()
        {
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockInputAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var mockOutputAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var mockSigner = new Mock<ISigner<HttpExecutionRequest>>();
            var mockLogger = new Mock<ILogger<JsonHttpExecutionAdapter>>();

            var execAdapter = new JsonHttpExecutionAdapter(
                mockLogger.Object,
                mockJsonHttpClient.Object,
                mockExecServiceProvider.Object,
                mockInputAccessorProvider.Object,
                mockOutputAccessorProvider.Object,
                mockSigner.Object);

            var execRequest = CreateDefaultExecutionRequest();

            execRequest.IsValidationSupported = true;
            execRequest.ValidateOnly = true;

            SetupDefaultMockExecutionServiceProvider(execRequest, mockExecServiceProvider);
            SetupDefaultMockInputObjectAccessorProvider(execRequest, mockInputAccessorProvider);
            SetupDefaultMockOutputObjectAccessorProvider(execRequest, mockOutputAccessorProvider);

            var httpExecResponse = new HttpExecutionResponse { ExecutionId = execRequest.ExecutionId };

            mockJsonHttpClient
               .Setup(hc => hc.PostAsync<HttpExecutionResponse>(defaultHttpExtensionSettings.ValidationUrl, It.IsAny<HttpExecutionRequest>()))
               .Returns(Task.FromResult(new HttpResponse<HttpExecutionResponse>(HttpStatusCode.OK, httpExecResponse)));

            var execContext = await execAdapter.ExecuteAsync(execRequest, CancellationToken.None);

            mockExecServiceProvider.Verify(sp => sp.OnValidatingAsync(execRequest));
        }

        [Fact]
        public async Task ExecuteAsync_ExecutionRequested_ShouldInvokeOnValidated()
        {
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockInputAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var mockOutputAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var mockSigner = new Mock<ISigner<HttpExecutionRequest>>();
            var mockLogger = new Mock<ILogger<JsonHttpExecutionAdapter>>();

            var execAdapter = new JsonHttpExecutionAdapter(
                mockLogger.Object,
                mockJsonHttpClient.Object,
                mockExecServiceProvider.Object,
                mockInputAccessorProvider.Object,
                mockOutputAccessorProvider.Object,
                mockSigner.Object);

            var execRequest = CreateDefaultExecutionRequest();

            execRequest.IsValidationSupported = true;
            execRequest.ValidateOnly = true;

            SetupDefaultMockExecutionServiceProvider(execRequest, mockExecServiceProvider);
            SetupDefaultMockInputObjectAccessorProvider(execRequest, mockInputAccessorProvider);
            SetupDefaultMockOutputObjectAccessorProvider(execRequest, mockOutputAccessorProvider);

            var httpExecResponse = new HttpExecutionResponse { ExecutionId = execRequest.ExecutionId };

            mockJsonHttpClient
               .Setup(hc => hc.PostAsync<HttpExecutionResponse>(defaultHttpExtensionSettings.ValidationUrl, It.IsAny<HttpExecutionRequest>()))
               .Returns(Task.FromResult(new HttpResponse<HttpExecutionResponse>(HttpStatusCode.OK, httpExecResponse)));

            var execContext = await execAdapter.ExecuteAsync(execRequest, CancellationToken.None);

            mockExecServiceProvider.Verify(sp =>
                sp.OnValidatedAsync(It.Is<Core.Models.ExecutionContext>(ec =>
                    (ec.Status == ExecutionStatus.ValidationSucceeded))));
        }

        [Fact]
        public async void ExecuteAsync_ExecutionRequested_ShouldPostExecutionRequestToExtension()
        {
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockInputAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var mockOutputAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var mockSigner = new Mock<ISigner<HttpExecutionRequest>>();
            var mockLogger = new Mock<ILogger<JsonHttpExecutionAdapter>>();

            var execAdapter = new JsonHttpExecutionAdapter(
                mockLogger.Object,
                mockJsonHttpClient.Object,
                mockExecServiceProvider.Object,
                mockInputAccessorProvider.Object,
                mockOutputAccessorProvider.Object,
                mockSigner.Object);

            var execRequest = CreateDefaultExecutionRequest();

            SetupDefaultMockExecutionServiceProvider(execRequest, mockExecServiceProvider);
            SetupDefaultMockInputObjectAccessorProvider(execRequest, mockInputAccessorProvider);
            SetupDefaultMockOutputObjectAccessorProvider(execRequest, mockOutputAccessorProvider);

            string executionUrl = null;
            HttpExecutionRequest httpExecRequest = null;

            execRequest.ExecutionParameters = JObject.FromObject(execRequest);

            var httpExecResponse = new HttpExecutionResponse { ExecutionId = execRequest.ExecutionId };

            mockJsonHttpClient
                .Setup(hc => hc.PostAsync<HttpExecutionResponse>(defaultHttpExtensionSettings.ExecutionUrl, It.IsAny<HttpExecutionRequest>()))
                .Returns(Task.FromResult(new HttpResponse<HttpExecutionResponse>(HttpStatusCode.OK, httpExecResponse)))
                .Callback<string, object>((u, er) =>
                {
                    executionUrl = u;
                    httpExecRequest = er as HttpExecutionRequest;
                });

            await execAdapter.ExecuteAsync(execRequest, CancellationToken.None);

            executionUrl.Should().NotBeNull();
            httpExecRequest.Should().NotBeNull();

            executionUrl.Should().Be(DefaultExtensionExecutionUrl);

            httpExecRequest.CreatedDateTimeUtc.Should().Be(execRequest.CreatedDateTimeUtc);
            httpExecRequest.ExecutionId.Should().Be(execRequest.ExecutionId);
            httpExecRequest.ExecutionProfileName.Should().Be(execRequest.ExecutionProfileName);
            httpExecRequest.Executor.Should().Be(execRequest.Executor);
            httpExecRequest.ExpirationDateTimeUtc.Should().Be(execRequest.ExecutionTimeoutDateTimeUtc);
            httpExecRequest.ExtensionId.Should().Be(execRequest.ExtensionId);
            httpExecRequest.ExtensionVersionId.Should().Be(execRequest.ExtensionVersionId);
            httpExecRequest.GetExecutionStatusUrl.Should().Be(execRequest.GetExecutionStatusUrl);
            httpExecRequest.Priority.Should().Be(execRequest.Priority);
            httpExecRequest.Services.Should().BeEquivalentTo(defaultExecutionServiceConfiguration);
            httpExecRequest.StatusUpdateKey.Should().Be(execRequest.StatusUpdateKey);
            httpExecRequest.UpdateExecutionStatusUrl.Should().Be(execRequest.UpdateExecutionStatusUrl);
            httpExecRequest.RequestParameters.Should().BeEquivalentTo(execRequest.ExecutionParameters);
        }

        [Fact]
        public async Task ExecuteAsync_ExecutionRequested_StatusShouldBeSucceededIfOk()
        {
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockInputAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var mockOutputAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var mockSigner = new Mock<ISigner<HttpExecutionRequest>>();
            var mockLogger = new Mock<ILogger<JsonHttpExecutionAdapter>>();

            var execAdapter = new JsonHttpExecutionAdapter(
                mockLogger.Object,
                mockJsonHttpClient.Object,
                mockExecServiceProvider.Object,
                mockInputAccessorProvider.Object,
                mockOutputAccessorProvider.Object,
                mockSigner.Object);

            var execRequest = CreateDefaultExecutionRequest();

            SetupDefaultMockExecutionServiceProvider(execRequest, mockExecServiceProvider);
            SetupDefaultMockInputObjectAccessorProvider(execRequest, mockInputAccessorProvider);
            SetupDefaultMockOutputObjectAccessorProvider(execRequest, mockOutputAccessorProvider);

            var httpExecResponse = new HttpExecutionResponse 
            { 
                ExecutionId = execRequest.ExecutionId,
                ResponseData = JObject.FromObject(execRequest)
            };

            mockJsonHttpClient
               .Setup(hc => hc.PostAsync<HttpExecutionResponse>(defaultHttpExtensionSettings.ExecutionUrl, It.IsAny<HttpExecutionRequest>()))
               .Returns(Task.FromResult(new HttpResponse<HttpExecutionResponse>(HttpStatusCode.OK, httpExecResponse)));

            var execContext = await execAdapter.ExecuteAsync(execRequest, CancellationToken.None);

            execContext.Status.Should().Be(ExecutionStatus.Succeeded);
            execContext.ResultData.Should().BeEquivalentTo(httpExecResponse.ResponseData);
        }

        [Fact]
        public async Task ExecuteAsync_ExecutionRequested_StatusShouldBeProcessingIfAccepted()
        {
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockInputAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var mockOutputAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var mockSigner = new Mock<ISigner<HttpExecutionRequest>>();
            var mockLogger = new Mock<ILogger<JsonHttpExecutionAdapter>>();

            var execAdapter = new JsonHttpExecutionAdapter(
                mockLogger.Object,
                mockJsonHttpClient.Object,
                mockExecServiceProvider.Object,
                mockInputAccessorProvider.Object,
                mockOutputAccessorProvider.Object,
                mockSigner.Object);

            var execRequest = CreateDefaultExecutionRequest();

            SetupDefaultMockExecutionServiceProvider(execRequest, mockExecServiceProvider);
            SetupDefaultMockInputObjectAccessorProvider(execRequest, mockInputAccessorProvider);
            SetupDefaultMockOutputObjectAccessorProvider(execRequest, mockOutputAccessorProvider);

            var httpExecResponse = new HttpExecutionResponse { ExecutionId = execRequest.ExecutionId };

            mockJsonHttpClient
               .Setup(hc => hc.PostAsync<HttpExecutionResponse>(defaultHttpExtensionSettings.ExecutionUrl, It.IsAny<HttpExecutionRequest>()))
               .Returns(Task.FromResult(new HttpResponse<HttpExecutionResponse>(HttpStatusCode.Accepted, httpExecResponse)));

            var execContext = await execAdapter.ExecuteAsync(execRequest, CancellationToken.None);

            execContext.Status.Should().Be(ExecutionStatus.Processing);
        }

        [Fact]
        public async Task ExecuteAsync_ExecutionRequested_StatusShouldValidationFailedIfBadRequestAndErrorsProvided()
        {
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockInputAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var mockOutputAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var mockSigner = new Mock<ISigner<HttpExecutionRequest>>();
            var mockLogger = new Mock<ILogger<JsonHttpExecutionAdapter>>();

            var execAdapter = new JsonHttpExecutionAdapter(
                mockLogger.Object,
                mockJsonHttpClient.Object,
                mockExecServiceProvider.Object,
                mockInputAccessorProvider.Object,
                mockOutputAccessorProvider.Object,
                mockSigner.Object);

            var execRequest = CreateDefaultExecutionRequest();

            SetupDefaultMockExecutionServiceProvider(execRequest, mockExecServiceProvider);
            SetupDefaultMockInputObjectAccessorProvider(execRequest, mockInputAccessorProvider);
            SetupDefaultMockOutputObjectAccessorProvider(execRequest, mockOutputAccessorProvider);

            var httpValidationError = new HttpExecutionValidationError
            {
                ErrorCode = "Wrong",
                ErrorId = CreateRandomId(),
                ErrorMessage = "You did it wrong."
            };

            var httpExecResponse = new HttpExecutionResponse
            {
                ExecutionId = execRequest.ExecutionId,
                ValidationErrors = new List<HttpExecutionValidationError> { httpValidationError }
            };

            mockJsonHttpClient
               .Setup(hc => hc.PostAsync<HttpExecutionResponse>(defaultHttpExtensionSettings.ExecutionUrl, It.IsAny<HttpExecutionRequest>()))
               .Returns(Task.FromResult(new HttpResponse<HttpExecutionResponse>(HttpStatusCode.BadRequest, httpExecResponse)));

            var execContext = await execAdapter.ExecuteAsync(execRequest, CancellationToken.None);

            execContext.Status.Should().Be(ExecutionStatus.ValidationFailed);
            execContext.ValidationErrors.Should().HaveCount(1);

            var validationError = execContext.ValidationErrors[0];

            validationError.ErrorCode.Should().Be(httpValidationError.ErrorCode);
            validationError.ErrorId.Should().Be(httpValidationError.ErrorId);
            validationError.ErrorMessage.Should().Be(httpValidationError.ErrorMessage);
        }

        [Fact]
        public async Task ExecuteAsync_ExecutionRequested_StatusShouldBeFailedIfBadRequestAndNoErrorsProvided()
        {
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockInputAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var mockOutputAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var mockSigner = new Mock<ISigner<HttpExecutionRequest>>();
            var mockLogger = new Mock<ILogger<JsonHttpExecutionAdapter>>();

            var execAdapter = new JsonHttpExecutionAdapter(
                mockLogger.Object,
                mockJsonHttpClient.Object,
                mockExecServiceProvider.Object,
                mockInputAccessorProvider.Object,
                mockOutputAccessorProvider.Object,
                mockSigner.Object);

            var execRequest = CreateDefaultExecutionRequest();

            SetupDefaultMockExecutionServiceProvider(execRequest, mockExecServiceProvider);
            SetupDefaultMockInputObjectAccessorProvider(execRequest, mockInputAccessorProvider);
            SetupDefaultMockOutputObjectAccessorProvider(execRequest, mockOutputAccessorProvider);

            var httpExecResponse = new HttpExecutionResponse { ExecutionId = execRequest.ExecutionId };

            mockJsonHttpClient
               .Setup(hc => hc.PostAsync<HttpExecutionResponse>(defaultHttpExtensionSettings.ExecutionUrl, It.IsAny<HttpExecutionRequest>()))
               .Returns(Task.FromResult(new HttpResponse<HttpExecutionResponse>(HttpStatusCode.BadRequest, httpExecResponse)));

            var execContext = await execAdapter.ExecuteAsync(execRequest, CancellationToken.None);

            execContext.Status.Should().Be(ExecutionStatus.Failed);
            execContext.ValidationErrors.Should().BeEmpty();
        }

        [Fact]
        public async Task ExecuteAsync_ExecutionRequested_StatusShouldBeFailedIfOtherStatusCode()
        {
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockInputAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var mockOutputAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var mockSigner = new Mock<ISigner<HttpExecutionRequest>>();
            var mockLogger = new Mock<ILogger<JsonHttpExecutionAdapter>>();

            var execAdapter = new JsonHttpExecutionAdapter(
                mockLogger.Object,
                mockJsonHttpClient.Object,
                mockExecServiceProvider.Object,
                mockInputAccessorProvider.Object,
                mockOutputAccessorProvider.Object,
                mockSigner.Object);

            var execRequest = CreateDefaultExecutionRequest();

            SetupDefaultMockExecutionServiceProvider(execRequest, mockExecServiceProvider);
            SetupDefaultMockInputObjectAccessorProvider(execRequest, mockInputAccessorProvider);
            SetupDefaultMockOutputObjectAccessorProvider(execRequest, mockOutputAccessorProvider);

            var httpExecResponse = new HttpExecutionResponse { ExecutionId = execRequest.ExecutionId };

            mockJsonHttpClient
               .Setup(hc => hc.PostAsync<HttpExecutionResponse>(defaultHttpExtensionSettings.ExecutionUrl, It.IsAny<HttpExecutionRequest>()))
               .Returns(Task.FromResult(new HttpResponse<HttpExecutionResponse>(HttpStatusCode.InternalServerError, httpExecResponse)));

            var execContext = await execAdapter.ExecuteAsync(execRequest, CancellationToken.None);

            execContext.Status.Should().Be(ExecutionStatus.Failed);
            execContext.ValidationErrors.Should().BeEmpty();
        }

        [Fact]
        public async Task ExecuteAsync_ExecutionRequested_ShouldInvokeOnExecuting()
        {
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockInputAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var mockOutputAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var mockSigner = new Mock<ISigner<HttpExecutionRequest>>();
            var mockLogger = new Mock<ILogger<JsonHttpExecutionAdapter>>();

            var execAdapter = new JsonHttpExecutionAdapter(
                mockLogger.Object,
                mockJsonHttpClient.Object,
                mockExecServiceProvider.Object,
                mockInputAccessorProvider.Object,
                mockOutputAccessorProvider.Object,
                mockSigner.Object);

            var execRequest = CreateDefaultExecutionRequest();

            SetupDefaultMockExecutionServiceProvider(execRequest, mockExecServiceProvider);
            SetupDefaultMockInputObjectAccessorProvider(execRequest, mockInputAccessorProvider);
            SetupDefaultMockOutputObjectAccessorProvider(execRequest, mockOutputAccessorProvider);

            var httpExecResponse = new HttpExecutionResponse { ExecutionId = execRequest.ExecutionId };

            mockJsonHttpClient
               .Setup(hc => hc.PostAsync<HttpExecutionResponse>(defaultHttpExtensionSettings.ExecutionUrl, It.IsAny<HttpExecutionRequest>()))
               .Returns(Task.FromResult(new HttpResponse<HttpExecutionResponse>(HttpStatusCode.OK, httpExecResponse)));

            var execContext = await execAdapter.ExecuteAsync(execRequest, CancellationToken.None);

            mockExecServiceProvider.Verify(sp => sp.OnExecutingAsync(execRequest));
        }

        [Fact]
        public async Task ExecuteAsync_ExecutionRequested_ShouldInvokeOnExecuted()
        {
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockExecServiceProvider = new Mock<IExecutionServiceProvider>();
            var mockInputAccessorProvider = new Mock<IInputObjectAccessorProvider>();
            var mockOutputAccessorProvider = new Mock<IOutputObjectAccessorProvider>();
            var mockSigner = new Mock<ISigner<HttpExecutionRequest>>();
            var mockLogger = new Mock<ILogger<JsonHttpExecutionAdapter>>();

            var execAdapter = new JsonHttpExecutionAdapter(
                mockLogger.Object,
                mockJsonHttpClient.Object,
                mockExecServiceProvider.Object,
                mockInputAccessorProvider.Object,
                mockOutputAccessorProvider.Object,
                mockSigner.Object);

            var execRequest = CreateDefaultExecutionRequest();

            SetupDefaultMockExecutionServiceProvider(execRequest, mockExecServiceProvider);
            SetupDefaultMockInputObjectAccessorProvider(execRequest, mockInputAccessorProvider);
            SetupDefaultMockOutputObjectAccessorProvider(execRequest, mockOutputAccessorProvider);

            var httpExecResponse = new HttpExecutionResponse { ExecutionId = execRequest.ExecutionId };

            mockJsonHttpClient
               .Setup(hc => hc.PostAsync<HttpExecutionResponse>(defaultHttpExtensionSettings.ExecutionUrl, It.IsAny<HttpExecutionRequest>()))
               .Returns(Task.FromResult(new HttpResponse<HttpExecutionResponse>(HttpStatusCode.OK, httpExecResponse)));

            var execContext = await execAdapter.ExecuteAsync(execRequest, CancellationToken.None);

            mockExecServiceProvider.Verify(sp =>
                sp.OnExecutedAsync(It.Is<Core.Models.ExecutionContext>(ec =>
                    (ec.Status == ExecutionStatus.Succeeded))));
        }

        private void SetupDefaultMockExecutionServiceProvider(ExecutionRequest execRequest,
                                                              Mock<IExecutionServiceProvider> mockExecServiceProvider)
        {
            mockExecServiceProvider.Setup(sp => sp.GetServiceConfigurationAsync(execRequest))
                                   .Returns(Task.FromResult(defaultExecutionServiceConfiguration));

            mockExecServiceProvider.Setup(sp => sp.OnExecutingAsync(It.IsAny<ExecutionRequest>()))
                                   .Returns(Task.CompletedTask);

            mockExecServiceProvider.Setup(sp => sp.OnExecutedAsync(It.IsAny<Core.Models.ExecutionContext>()))
                                   .Returns(Task.CompletedTask);

            mockExecServiceProvider.Setup(sp => sp.OnValidatingAsync(It.IsAny<ExecutionRequest>()))
                                   .Returns(Task.CompletedTask);

            mockExecServiceProvider.Setup(sp => sp.OnValidatedAsync(It.IsAny<Core.Models.ExecutionContext>()))
                                   .Returns(Task.CompletedTask);
        }

        private void SetupDefaultMockInputObjectAccessorProvider(ExecutionRequest execRequest,
                                                                 Mock<IInputObjectAccessorProvider> mockInputAccessorProvider)
        {
            foreach (var inputObjectName in inputObjectNames)
            {
                mockInputAccessorProvider
                    .Setup(ap => ap.GetReadableAccessorAsync(It.Is<InputObjectAccessorRequest>(ar =>
                        ar.ExecutionMetadata == execRequest &&
                        ar.ObjectMetadata == execRequest.InputObjects[inputObjectName] &&
                        ar.ObjectProviderName == DefaultObjectProviderName)))
                    .Returns(Task.FromResult(defaultInputObjectAccessorDictionary[inputObjectName]));
            }
        }

        private void SetupDefaultMockOutputObjectAccessorProvider(ExecutionRequest execRequest,
                                                                  Mock<IOutputObjectAccessorProvider> mockOutputAccessorProvider)
        {
            foreach (var outputObjectName in outputObjectNames)
            {
                mockOutputAccessorProvider
                    .Setup(ap => ap.GetWritableAccessorAsync(It.Is<OutputObjectAccessorRequest>(ar =>
                        ar.ExecutionMetadata == execRequest &&
                        ar.ObjectMetadata == execRequest.OutputObjects[outputObjectName] &&
                        ar.ObjectProviderName == DefaultObjectProviderName)))
                    .Returns(Task.FromResult(defaultOutputObjectAccessorDictionary[outputObjectName]));
            }
        }

        private ExecutionRequest CreateDefaultExecutionRequest() =>
            new ExecutionRequest
            {
                CreatedDateTimeUtc = DateTime.UtcNow,
                ExecutionId = CreateRandomId(),
                ExecutionModelName = DefaultExecutionModelName,
                ExecutionProfileName = DefaultExecutionProfileName,
                ExecutionTimeoutDateTimeUtc = DateTime.UtcNow.AddHours(1),
                Executor = new ExecutorContext
                {
                    TenantId = CreateRandomId(),
                    UserId = CreateRandomId(),
                },
                ExtensionId = Guid.NewGuid().ToString(),
                ExtensionSettings = JObject.FromObject(defaultHttpExtensionSettings),
                ExtensionVersionId = CreateRandomId(),
                GetExecutionStatusUrl = DefaultGetExecutionStatusUrl,
                LastUpdatedDateTimeUtc = DateTime.UtcNow,
                ObjectProviderName = DefaultObjectProviderName,
                Priority = ExecutionPriority.High,
                StatusUpdateKey = CreateRandomId(),
                UpdateExecutionStatusUrl = DefaultUpdateExecutionStatusUrl,
                InputObjects = defaultInputObjectDictionary,
                OutputObjects = defaultOutputObjectDictionary,
                ProvidedInputObjects = inputObjectNames.ToList()
            };

        private HttpExtensionSettings CreateDefaultHttpExtensionSettings() =>
            new HttpExtensionSettings
            {
                ExecutionUrl = DefaultExtensionExecutionUrl,
                ValidationUrl = DefaultExtensionValidationUrl
            };

        private JObject CreateDefaultExecutionServiceConfiguration() => JObject.FromObject(
            new Dictionary<string, string>
            {
                { "Service A", "Setting A" },
                { "Service B", "Setting B" },
                { "Service C", "Setting C" }
            });

        private Dictionary<string, ExtensionInputObject> CreateDefaultInputObjectDictionary() =>
            inputObjectNames.ToDictionary(n => n, n => new ExtensionInputObject { Name = n });

        private Dictionary<string, ExtensionOutputObject> CreateDefaultOutputObjectDictionary() =>
            outputObjectNames.ToDictionary(n => n, n => new ExtensionOutputObject { Name = n });

        private Dictionary<string, JObject> CreateDefaultInputObjectAccessorDictionary() =>
            inputObjectNames.ToDictionary(n => n, n => JObject.FromObject(
                new Dictionary<string, string> { { "Name", n } }));

        private Dictionary<string, JObject> CreateDefaultOutputObjectAccessorDictionary() =>
            outputObjectNames.ToDictionary(n => n, n => JObject.FromObject(
                new Dictionary<string, string> { { "Name", n } }));

        private string CreateRandomId() => Guid.NewGuid().ToString();
    }
}
