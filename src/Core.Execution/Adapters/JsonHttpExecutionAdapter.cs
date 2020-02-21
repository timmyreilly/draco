// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Extensions;
using Draco.Core.Execution.Interfaces;
using Draco.Core.Execution.Models;
using Draco.Core.Interfaces;
using Draco.Core.Models;
using Draco.Core.Models.Enumerations;
using Draco.Core.Models.Extensions;
using Draco.Core.ObjectStorage.Interfaces;
using Draco.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Draco.Core.Execution.Adapters
{
    public class JsonHttpExecutionAdapter : BaseExecutionAdapter, IExecutionAdapter
    {
        private readonly TimeSpan defaultTimeoutPeriod = TimeSpan.FromHours(1);

        private readonly ILogger logger;
        private readonly IJsonHttpClient jsonHttpClient;
        private readonly IExecutionServiceProvider execServiceProvider;
        private readonly ISigner<HttpExecutionRequest> httpExecRequestSigner;

        public JsonHttpExecutionAdapter(
            ILogger<JsonHttpExecutionAdapter> logger,
            IJsonHttpClient jsonHttpClient,
            IExecutionServiceProvider execServiceProvider,
            IInputObjectAccessorProvider inputObjectAccessorProvider,
            IOutputObjectAccessorProvider outputObjectAccessorProvider,
            ISigner<HttpExecutionRequest> httpExecRequestSigner)
            : base(inputObjectAccessorProvider, outputObjectAccessorProvider)
        {
            this.logger = logger;
            this.jsonHttpClient = jsonHttpClient;
            this.execServiceProvider = execServiceProvider;
            this.httpExecRequestSigner = httpExecRequestSigner;
        }

        public async Task<Core.Models.ExecutionContext> ExecuteAsync(ExecutionRequest execRequest, CancellationToken cancelToken)
        {
            if (execRequest == null)
            {
                throw new ArgumentNullException(nameof(execRequest));
            }

            logger.LogInformation($"Processing execution request [{execRequest.ExecutionId}]...");

            await (execRequest.ValidateOnly ?
                   this.execServiceProvider.OnValidatingAsync(execRequest) :
                   this.execServiceProvider.OnExecutingAsync(execRequest));

            execRequest.CalculateExecutionTimeoutDateTimeUtc(defaultTimeoutPeriod);

            logger.LogDebug($"Execution [{execRequest.ExecutionId}] timeout set to [{execRequest.ExecutionTimeoutDateTimeUtc}] UTC.");

            var execContext = execRequest.ToExecutionContext();

            try
            {
                var httpSettings = GetHttpExtensionSettings(execRequest);
                var httpExecRequest = await ToHttpExecutionRequestAsync(execRequest);

                logger.LogDebug($"Execution [{execRequest.ExecutionId}] URL is [{httpSettings.ExecutionUrl}].");

                logger.LogDebug(string.IsNullOrEmpty(httpSettings.ValidationUrl) ?
                                $"Execution [{execRequest.ExecutionId}] validation URL is [{httpSettings.ValidationUrl}]." :
                                $"Execution [{execRequest.ExecutionId}] validation URL not provided.");

                if (!string.IsNullOrEmpty(execRequest.SignatureRsaKeyXml))
                {
                    logger.LogDebug($"Signing execution request [{execRequest.ExecutionId}]...");

                    httpExecRequest.Signature = await httpExecRequestSigner.GenerateSignatureAsync(execRequest.SignatureRsaKeyXml, httpExecRequest);
                }

                if (execRequest.ValidateOnly)
                {
                    if (execRequest.IsValidationSupported == false)
                    {
                        throw new NotSupportedException($"Extension [{execRequest.ExtensionId}:{execRequest.ExtensionVersionId}] " +
                                                         "does not support validation.");
                    }

                    execContext = await ValidateAsync(execContext, httpExecRequest, httpSettings);
                }
                else
                {
                    execContext = await ExecuteAsync(execContext, httpExecRequest, httpSettings);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while processing execution request [{execRequest.ExecutionId}]: [{ex.Message}].");

                execContext.UpdateStatus(ExecutionStatus.Failed);
            }
            finally
            {
                await (execRequest.ValidateOnly ?
                       this.execServiceProvider.OnValidatedAsync(execContext) :
                       this.execServiceProvider.OnExecutedAsync(execContext));

                logger.LogInformation($"Execution request [{execRequest.ExecutionId}] processing complete.");
            }

            return execContext;
        }

        private async Task<Core.Models.ExecutionContext> ExecuteAsync<T>(Core.Models.ExecutionContext execContext, T httpExecRequest,
                                                                         HttpExtensionSettings httpSettings)
        {
            logger.LogInformation($"Posting execution request [{execContext.ExecutionId}] to [{httpSettings.ExecutionUrl}]...");

            var httpExecResponse = await this.jsonHttpClient.PostAsync<HttpExecutionResponse>(
                httpSettings.ExecutionUrl, httpExecRequest);

            execContext.ResultData = httpExecResponse.Content?.ResponseData;
            execContext.ProvidedOutputObjects = httpExecResponse.Content?.ProvidedOutputObjects;
            execContext.ValidationErrors = httpExecResponse.Content?.ValidationErrors?.Select(ve => ve.ToCoreModel()).ToList();

            switch (httpExecResponse.StatusCode)
            {
                case HttpStatusCode.Accepted:
                    logger.LogInformation($"Execution [{execContext.ExecutionId}] is long-running.");
                    execContext.UpdateStatus(ExecutionStatus.Processing);
                    break;
                case HttpStatusCode.OK:
                    logger.LogInformation($"Execution [{execContext.ExecutionId}] complete.");
                    execContext.UpdateStatus(ExecutionStatus.Succeeded);
                    break;
                case HttpStatusCode.BadRequest:
                    logger.LogWarning($"Execution request [{execContext.ExecutionId}] is invalid.");
                    ProcessBadRequest(execContext, httpExecResponse.Content);
                    break;
                default:
                    throw new HttpRequestException($"Extension returned an unexpected status code: [{httpExecResponse.StatusCode}].");
            }

            return execContext;
        }
        private async Task<Core.Models.ExecutionContext> ValidateAsync<T>(Core.Models.ExecutionContext execContext, T httpExecRequest,
                                                                          HttpExtensionSettings httpSettings)

        {
            logger.LogInformation($"Posting execution validation request [{execContext.ExecutionId}] to [{httpSettings.ValidationUrl}]...");

            var httpExecResponse = await this.jsonHttpClient.PostAsync<HttpExecutionResponse>(
                httpSettings.ValidationUrl, httpExecRequest);

            execContext.ValidationErrors = httpExecResponse.Content?.ValidationErrors?.Select(ve => ve.ToCoreModel()).ToList();

            switch (httpExecResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    logger.LogInformation($"Execution request [{execContext.ExecutionId}] is valid.");
                    execContext.UpdateStatus(ExecutionStatus.ValidationSucceeded);
                    break;
                case HttpStatusCode.BadRequest:
                    logger.LogInformation($"Execution request [{execContext.ExecutionId}] is invalid.");
                    ProcessBadRequest(execContext, httpExecResponse.Content);
                    break;
                default:
                    throw new HttpRequestException($"Extension returned an unexpected status code: [{httpExecResponse.StatusCode}].");
            }

            return execContext;
        }

        private Core.Models.ExecutionContext ProcessBadRequest(Core.Models.ExecutionContext execContext,
                                                               HttpExecutionResponse httpExecResponse)
        {
            execContext.ValidationErrors = httpExecResponse?.ValidationErrors.Select(e => e.ToCoreModel()).ToList();

            if (execContext.ValidationErrors.Any())
            {
                return execContext.UpdateStatus(ExecutionStatus.ValidationFailed);
            }
            else
            {
                throw new HttpRequestException($"Extension returned an unexpected status code: [{HttpStatusCode.BadRequest}].");
            }
        }

        private HttpExtensionSettings GetHttpExtensionSettings(ExecutionRequest execRequest) =>
             execRequest.ExtensionSettings.ToObject<HttpExtensionSettings>();

        private async Task<HttpExecutionRequest> ToHttpExecutionRequestAsync(ExecutionRequest execRequest) => new HttpExecutionRequest
        {
            CreatedDateTimeUtc = execRequest.CreatedDateTimeUtc,
            ExecutionId = execRequest.ExecutionId,
            Executor = execRequest.Executor,
            ExtensionId = execRequest.ExtensionId,
            ExtensionVersionId = execRequest.ExtensionVersionId,
            GetExecutionStatusUrl = execRequest.GetExecutionStatusUrl,
            LastUpdatedDateTimeUtc = execRequest.LastUpdatedDateTimeUtc,
            Priority = execRequest.Priority,
            StatusUpdateKey = execRequest.StatusUpdateKey,
            UpdateExecutionStatusUrl = execRequest.UpdateExecutionStatusUrl,
            ExpirationDateTimeUtc = execRequest.ExecutionTimeoutDateTimeUtc,
            Services = await execServiceProvider.GetServiceConfigurationAsync(execRequest),
            InputObjects = await CreateInputObjectAccessorDictionaryAsync(execRequest),
            OutputObjects = await CreateOutputObjectAccessorDictionaryAsync(execRequest),
            ExecutionProfileName = execRequest.ExecutionProfileName,
            RequestParameters = execRequest.ExecutionParameters,
            ExecutorProperties = execRequest.ExecutorProperties
        };
    }
}
