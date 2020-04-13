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
    /// <summary>
    /// This execution adapter supports the "json-http/async/v1" and "json-http/sync/v1" execution models.
    /// For more information, see /doc/architecture/execution-models.md#example.
    /// </summary>
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

            // Give all the applicable extension services a chance to do any synchronous pre-work ahead of the actual execution...
            // For more information on extension services, see /doc/architecture/extension-services.md.

            await (execRequest.ValidateOnly ?
                   this.execServiceProvider.OnValidatingAsync(execRequest) :
                   this.execServiceProvider.OnExecutingAsync(execRequest));

            // UTC now + the execution timeout on the execution request if it was provided.
            // If no execution timeout was defined, default to an hour [defaultTimeoutPeriod].

            execRequest.CalculateExecutionTimeoutDateTimeUtc(defaultTimeoutPeriod);

            logger.LogDebug($"Execution [{execRequest.ExecutionId}] timeout set to [{execRequest.ExecutionTimeoutDateTimeUtc}] UTC.");

            // We're about to execution the extension. Create an execution context to capture the results...

            var execContext = execRequest.ToExecutionContext();

            try
            {
                // Deserialize the execution model-specific settings, provided in the [execRequest.extensionSettings] JSON object property,
                // to get the execution and/or validation URLs that we need to handle the request.

                var httpSettings = GetHttpExtensionSettings(execRequest);

                // Create the execution request that we'll POST to the extension...

                var httpExecRequest = await ToHttpExecutionRequestAsync(execRequest);

                logger.LogDebug($"Execution [{execRequest.ExecutionId}] URL is [{httpSettings.ExecutionUrl}].");

                logger.LogDebug(string.IsNullOrEmpty(httpSettings.ValidationUrl) ?
                                $"Execution [{execRequest.ExecutionId}] validation URL is [{httpSettings.ValidationUrl}]." :
                                $"Execution [{execRequest.ExecutionId}] validation URL not provided.");

                // If there was a valid RSA public/private key pair provided along with the request,
                // sign the request before we send it along to the extension...

                if (!string.IsNullOrEmpty(execRequest.SignatureRsaKeyXml))
                {
                    logger.LogDebug($"Signing execution request [{execRequest.ExecutionId}]...");

                    httpExecRequest.Signature = await httpExecRequestSigner.GenerateSignatureAsync(execRequest.SignatureRsaKeyXml, httpExecRequest);
                }

                if (execRequest.ValidateOnly)
                {
                    // If it's a validation only request, make sure that the extension actually supports validation.
                    // Under normal circumstances, this should have been caught way further upstream at the execution API.

                    if (execRequest.IsValidationSupported == false)
                    {
                        throw new NotSupportedException($"Extension [{execRequest.ExtensionId}:{execRequest.ExtensionVersionId}] " +
                                                         "does not support validation.");
                    }

                    // Validate the request...

                    execContext = await ValidateAsync(execContext, httpExecRequest, httpSettings);
                }
                else
                {
                    // Execute the request...

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
                // Give all the applicable extension services a chance to do any synchronous post-work after the execution
                // regardless of whether or not it succeeded. For more information on extension services, see /doc/architecture/extension-services.md.

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

            // POST the request to the extension...

            var httpExecResponse = await this.jsonHttpClient.PostAsync<HttpExecutionResponse>(
                httpSettings.ExecutionUrl, httpExecRequest);

            // Gather all the appropriate information returned from the extension...

            execContext.ResultData = httpExecResponse.Content?.ResponseData;
            execContext.ProvidedOutputObjects = httpExecResponse.Content?.ProvidedOutputObjects;
            execContext.ValidationErrors = httpExecResponse.Content?.ValidationErrors?.Select(ve => ve.ToCoreModel()).ToList();

            switch (httpExecResponse.StatusCode)
            {
                // If the extension responded with a [202 Accepted], the execution is long-running. Mark the execution as [Processing].
                // The extension is expected to call back to the execution API with status updates.
                case HttpStatusCode.Accepted:
                    logger.LogInformation($"Execution [{execContext.ExecutionId}] is long-running.");
                    execContext.UpdateStatus(ExecutionStatus.Processing);
                    break;

                // If the extension responded with a [200 OK], execution was succesful. Mark the execution as such. 
                case HttpStatusCode.OK:
                    logger.LogInformation($"Execution [{execContext.ExecutionId}] complete.");
                    execContext.UpdateStatus(ExecutionStatus.Succeeded);
                    break;

                // If the extension responded with a [400 Bad Request], either we called the extension wrong OR the client
                // called the extension wrong. The extension may have provided further information as [validationErrors].
                // If [validationErrors] were provided, mark the execution as [ValidationFailed].
                // If [validationErrors] were not provided, something else went wrong, so mark the execution as [Failed].
                case HttpStatusCode.BadRequest:
                    logger.LogWarning($"Execution request [{execContext.ExecutionId}] is invalid.");
                    ProcessBadRequest(execContext, httpExecResponse.Content);
                    break;

                // The extension responded with a status code that we didn't expect. Throw an exception...
                default:
                    throw new HttpRequestException($"Extension returned an unexpected status code: [{httpExecResponse.StatusCode}].");
            }

            return execContext;
        }
        private async Task<Core.Models.ExecutionContext> ValidateAsync<T>(Core.Models.ExecutionContext execContext, T httpExecRequest,
                                                                          HttpExtensionSettings httpSettings)

        {
            logger.LogInformation($"Posting execution validation request [{execContext.ExecutionId}] to [{httpSettings.ValidationUrl}]...");

            // POST the validation request to the extension...

            var httpExecResponse = await this.jsonHttpClient.PostAsync<HttpExecutionResponse>(
                httpSettings.ValidationUrl, httpExecRequest);

            // Grab any validation errors...

            execContext.ValidationErrors = httpExecResponse.Content?.ValidationErrors?.Select(ve => ve.ToCoreModel()).ToList();

            switch (httpExecResponse.StatusCode)
            {
                // If the extension responded with a [200 OK], mark the execution [ValidationSucceeded].
                case HttpStatusCode.OK:
                    logger.LogInformation($"Execution request [{execContext.ExecutionId}] is valid.");
                    execContext.UpdateStatus(ExecutionStatus.ValidationSucceeded);
                    break;

                // If the extension responded with a [400 Bad Request], either we called the extension wrong OR the client
                // called the extension wrong. The extension may have provided further information as [validationErrors].
                // If [validationErrors] were provided, mark the execution as [ValidationFailed].
                // If [validationErrors] were not provided, something else went wrong, so mark the execution as [Failed].
                case HttpStatusCode.BadRequest:
                    logger.LogInformation($"Execution request [{execContext.ExecutionId}] is invalid.");
                    ProcessBadRequest(execContext, httpExecResponse.Content);
                    break;

                // The extension responded with a status code that we didn't expect. Throw an exception...
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
