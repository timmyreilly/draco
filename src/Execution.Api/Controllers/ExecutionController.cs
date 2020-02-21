// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Extensions;
using Draco.Core.Execution.Interfaces;
using Draco.Core.Extensions;
using Draco.Core.Interfaces;
using Draco.Core.Models;
using Draco.Core.Models.Enumerations;
using Draco.Core.Models.Extensions;
using Draco.Core.Models.Interfaces;
using Draco.Core.Services.Interfaces;
using Draco.Execution.Api.Constants;
using Draco.Execution.Api.Extensions;
using Draco.Execution.Api.Interfaces;
using Draco.Execution.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Draco.Execution.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class ExecutionController : ControllerBase
    {
        private readonly IExecutionRequestContextBuilder erContextBuilder;
        private readonly IExecutionRepository execRepository;
        private readonly IExtensionRepository extensionRepository;
        private readonly IExtensionObjectApiModelService extensionObjectApiModelService;
        private readonly IExtensionRsaKeyProvider extensionRsaKeyProvider;
        private readonly IExtensionSettingsBuilder extensionSettingsBuilder;
        private readonly IExecutionRequestRouter execRequestRouter;
        private readonly IExecutionServiceProvider execServiceProvider;
        private readonly IExecutionUpdatePublisher execUpdatePublisher;
        private readonly ISigner<DirectExecutionRequestApiModel> directExecRequestSigner;
        private readonly IUserContext userContext;

        public ExecutionController(
            IExecutionRequestContextBuilder erContextBuilder,
            IExecutionRepository execRepository,
            IExtensionRepository extensionRepository,
            IExtensionObjectApiModelService extensionObjectApiModelService,
            IExtensionRsaKeyProvider extensionRsaKeyProvider,
            IExtensionSettingsBuilder extensionSettingsBuilder,
            IExecutionRequestRouter execRequestRouter,
            IExecutionUpdatePublisher execUpdatePublisher,
            IExecutionServiceProvider execServiceProvider,
            ISigner<DirectExecutionRequestApiModel> directExecRequestSigner,
            IUserContext userContext)
        {
            this.erContextBuilder = erContextBuilder;
            this.execRepository = execRepository;
            this.extensionRepository = extensionRepository;
            this.extensionObjectApiModelService = extensionObjectApiModelService;
            this.extensionRsaKeyProvider = extensionRsaKeyProvider;
            this.extensionSettingsBuilder = extensionSettingsBuilder;
            this.execRequestRouter = execRequestRouter;
            this.execUpdatePublisher = execUpdatePublisher;
            this.userContext = userContext;
            this.execServiceProvider = execServiceProvider;
            this.directExecRequestSigner = directExecRequestSigner;
        }

        /// <summary>
        /// Creates a new execution
        /// </summary>
        /// <param name="apiExecRequest">The execution request</param>
        /// <response code="200">Either the execution has completed synchronously or, in the case of direct execution, an execution token has been returned. Details included in response.</response>
        /// <response code="202">The execution is queued or input objects (specified in the response) are required before continuing. Details included in response.</response>
        /// <response code="400">Details included in response.</response>
        [HttpPost]
        public async Task<IActionResult> CreateNewExecutionAsync([FromBody, Required] ExecutionRequestApiModel apiExecRequest)
        {
            var erContext = await erContextBuilder.BuildExecutionRequestContextAsync(apiExecRequest);

            if (erContext.ValidationErrors.Any())
            {
                return BadRequest($"[{erContext.ValidationErrors.Count}] error(s) were encountered while attempting to fulfill " +
                                  $"your request. -- {erContext.ValidationErrors.ToSpaceSeparatedString()}");
            }

            erContext.Execution = await ToExecutionAsync(erContext);

            if (erContext.ExtensionVersion.InputObjects.Any())
            {
                erContext.Execution.Status = ExecutionStatus.PendingInputObjects;

                await UpdateExecutionAsync(erContext.Execution);

                return Accepted(await CreateToContinueApiModelAsync(erContext, "Please provide the following objects before continuing."));
            }
            else
            {
                if (erContext.ExecutionProfile.ExecutionMode == ExecutionMode.Direct)
                {
                    return Ok(await CreateDirectExecutionRequestAsync(erContext));
                }
                else
                {
                    return await ToExecutionRequestRoutedResultAsync(await ExecuteAsync(erContext));
                }
            }
        }

        /// <summary>
        /// Continues existing execution after input object(s) have been provided
        /// </summary>
        /// <param name="executionId">The execution ID</param>
        /// <param name="continueApiModel">A list of input object(s) that have been provided</param>
        /// <response code="200">The execution has completed synchronously. Details included in response.</response>
        /// <response code="202">The execution has been queued. Details included in response.</response>
        /// <response code="400">Details included in response.</response>
        /// <response code="404">The specified execution was not found.</response>
        [HttpPut("{executionId}/continue")]
        [ProducesResponseType(typeof(ExecutionUpdateApiModel), 200)]
        [ProducesResponseType(typeof(ExecutionUpdateApiModel), 202)]
        public async Task<IActionResult> ContinueExecutionAsync([Required] string executionId, 
                                                                [Required, FromBody] ContinueExecutionApiModel continueApiModel)
        {
            var erContext = new ExecutionRequestContext<ContinueExecutionApiModel>(continueApiModel)
            {
                Execution = await execRepository.GetExecutionAsync(executionId, userContext.Executor.TenantId)
            };

            if (erContext.Execution == null)
            {
                return NotFound($"[{ErrorCodes.ExecutionNotFound}]: Execution [{executionId}] not found.");
            }

            if (erContext.Execution.Status != ExecutionStatus.PendingInputObjects)
            {
                return BadRequest($"[{ErrorCodes.UnableToContinue}]: Unable to continue; " +
                                  $"execution [{executionId}] is already [{erContext.Execution.Status}].");
            }

            erContext.Extension = await extensionRepository.GetExtensionAsync(erContext.Execution.ExtensionId);
            erContext.ExtensionVersion = erContext.Extension.GetExtensionVersion(erContext.Execution.ExtensionVersionId);
            erContext.ExecutionProfile = erContext.ExtensionVersion.GetExecutionProfile(erContext.Execution.ExecutionProfileName);

            erContext.Execution.ProvidedInputObjects.AddRange(erContext.OriginalRequest.ProvidedInputObjects.Where(pio =>
                erContext.ExtensionVersion.InputObjects.Any(io => io.Name == pio) &&
                erContext.Execution.ProvidedInputObjects.Contains(pio) == false));

            var unknownObjects = erContext.OriginalRequest.ProvidedInputObjects
                .Where(pio => erContext.Execution.InputObjects.Select(io => io.Name).Contains(pio) == false)
                .ToArray();

            var neededObjects = erContext.Execution.InputObjects
                .Where(io => io.IsRequired && (erContext.OriginalRequest.ProvidedInputObjects.Contains(io.Name) == false))
                .Select(io => io.Name)
                .ToArray();

            if (unknownObjects.Any() || neededObjects.Any())
            {
                var errorMessages = new List<string>();

                if (unknownObjects.Any())
                {
                    errorMessages.Add($"[{ErrorCodes.UnknownInputObjects}]: " +
                                      $"The following provided input objects are unknown: [{unknownObjects.ToCommaSeparatedString()}].");
                }

                if (neededObjects.Any())
                {
                    errorMessages.Add($"[{ErrorCodes.MissingInputObjects}]: " +
                                      $"The following input objects are missing: [{neededObjects.ToCommaSeparatedString()}].");
                }

                return BadRequest(await CreateToContinueApiModelAsync(erContext, errorMessages.ToSpaceSeparatedString()));
            }
            else
            {
                return await ToExecutionRequestRoutedResultAsync(await ExecuteAsync(erContext));
            }
        }

        /// <summary>
        /// Gets the status of an existing execution
        /// </summary>
        /// <param name="executionId">The execution ID</param>
        /// <returns></returns>
        /// <status code="200">Current execution status provided.</status>
        /// <status code="404">Specified execution not found.</status>
        [HttpGet("{executionId}/status")]
        [ProducesResponseType(typeof(ExecutionUpdateApiModel), 200)]
        public async Task<IActionResult> GetExecutionStatusAsync([Required] string executionId)
        {
            var execution = await execRepository.GetExecutionAsync(executionId, userContext.Executor.TenantId);

            if (execution == null)
            {
                return NotFound($"[{ErrorCodes.ExecutionNotFound}]: Execution [{executionId}] not found.");
            }

            return Ok(await ToExecutionUpdateApiModelAsync(execution));
        }

        /// <summary>
        /// Updates the status of an existing execution
        /// </summary>
        /// <param name="executionId">The execution ID</param>
        /// <param name="updateApiModel">The execution status update</param>
        /// <returns></returns>
        /// <status code="200">Execution status successfully updated. Current status provided.</status>
        /// <status code="400">Details included in response.</status>
        /// <status code="403">The provided status update key is invalid.</status>
        /// <status code="404">Specified execution not found.</status>
        [HttpPut("{executionId}/status")]
        [ProducesResponseType(typeof(ExecutionUpdateApiModel), 200)]
        public async Task<IActionResult> UpdateExecutionStatusAsync([Required] string executionId, 
                                                                    [Required, FromBody] UpdateExecutionStatusApiModel updateApiModel)
        {
            var execution = await execRepository.GetExecutionAsync(executionId, userContext.Executor.TenantId);

            if (execution == null)
            {
                return NotFound($"[{ErrorCodes.ExecutionNotFound}]: Execution [{executionId}] not found.");
            }

            if (execution.StatusUpdateKey != updateApiModel.StatusUpdateKey)
            {
                return Forbid();
            }

            execution = ApplyExecutionUpdate(updateApiModel, execution);

            if (string.IsNullOrEmpty(updateApiModel.Status) == false)
            {
                if (Enum.TryParse<ExecutionStatus>(updateApiModel.Status, out var execStatus))
                {
                    if (execution.Status.CanTransitionTo(execStatus))
                    {
                        execution.Status = execStatus;
                    }
                    else
                    {
                        return BadRequest($"[{ErrorCodes.InvalidExecutionStatusTransition}]: " +
                                          $"Unable to update execution state to [{updateApiModel.Status}]; " +
                                          $"execution is already [{updateApiModel.Status}].");
                    }
                }
                else
                {
                    return BadRequest($"[{ErrorCodes.InvalidExecutionStatus}]: Execution status [{updateApiModel.Status}] is invalid.");
                }
            }

            await UpdateExecutionAsync(execution);

            return Ok(await ToExecutionUpdateApiModelAsync(execution));
        }

        private async Task<DirectExecutionRequestApiModel> CreateDirectExecutionRequestAsync(IExecutionRequestContext erContext)
        {
            var execRequest = await ToExecutionRequestAsync(erContext);

            var directExecRequest = new DirectExecutionRequestApiModel
            {
                ExecutionId = execRequest.ExecutionId,
                ExecutionModelName = execRequest.ExecutionModelName,
                ExecutionProfileName = execRequest.ExecutionProfileName,
                ExecutionSettings = execRequest.ExtensionSettings,
                ExtensionId = execRequest.ExtensionId,
                ExtensionVersionId = execRequest.ExtensionVersionId,
                GetExecutionStatusUrl = execRequest.GetExecutionStatusUrl,
                ObjectProviderName = execRequest.ObjectProviderName,
                ExecutorProperties = execRequest.ExecutorProperties,
                Services = await execServiceProvider.GetServiceConfigurationAsync(execRequest)
            };

            directExecRequest.ExpirationDateTimeUtc = DateTime.UtcNow.Add(erContext.ExecutionProfile.DirectExecutionTokenDuration.Value);
            directExecRequest.Signature = await directExecRequestSigner.GenerateSignatureAsync(execRequest.SignatureRsaKeyXml, directExecRequest);

            erContext.Execution.ExecutionTimeoutDateTimeUtc = directExecRequest.ExpirationDateTimeUtc;
            erContext.Execution.Status = ExecutionStatus.DirectExecutionTokenProvided;
            erContext.Execution.LastUpdatedDateTimeUtc = DateTime.UtcNow;

            await UpdateExecutionAsync(erContext.Execution);

            return directExecRequest;
        }

        private async Task<Core.Models.Execution> ExecuteAsync(IExecutionRequestContext erContext)
        {
            var execRequest = await ToExecutionRequestAsync(erContext);
            var execContext = await execRequestRouter.RouteRequestAsync(execRequest, CancellationToken.None);

            erContext.Execution = ApplyExecutionContext(execContext, erContext.Execution);

            await UpdateExecutionAsync(erContext.Execution);

            return erContext.Execution;
        }

        private async Task UpdateExecutionAsync(Core.Models.Execution execution)
        {
            await execRepository.UpsertExecutionAsync(execution);
            await execUpdatePublisher.PublishUpdateAsync(execution);
        }

        private async Task<IActionResult> ToExecutionRequestRoutedResultAsync(Core.Models.Execution execution)
        {
            var updateApiModel = await ToExecutionUpdateApiModelAsync(execution);

            switch (execution.Status)
            {
                case ExecutionStatus.Processing:
                case ExecutionStatus.Queued:
                    return Accepted(updateApiModel);
                case ExecutionStatus.Canceled:
                case ExecutionStatus.Succeeded:
                case ExecutionStatus.ValidationSucceeded:
                    return Ok(updateApiModel);
                case ExecutionStatus.ValidationFailed:
                    return BadRequest(updateApiModel);
                default:
                    return StatusCode((int)(HttpStatusCode.InternalServerError), updateApiModel);
            }
        }

        private async Task<ToContinueExecutionApiModel> CreateToContinueApiModelAsync(IExecutionRequestContext erContext, string message = null) =>
            new ToContinueExecutionApiModel
            {
                ExecutionId = erContext.Execution.ExecutionId,
                ExecutionStatus = erContext.Execution.Status.ToString(),
                Message = message,
                PutContinueExecutionUrl = GetContinueExecutionUrl(erContext.Execution.ExecutionId),
                GetExecutionStatusUrl = GetExecutionStatusUrl(erContext.Execution.ExecutionId),
                ExecutorProperties = erContext.Execution.ExecutorProperties,
                InputObjects = await extensionObjectApiModelService.CreateInputObjectDictionaryAsync(
                    erContext.Execution.InputObjects, 
                    erContext.Execution)
            };

        private string GetContinueExecutionUrl(string executionId) =>
            (Request == null) ? default : $"{Request.Scheme}://{Request.Host}/execution/{executionId}/continue";

        private string GetExecutionStatusUrl(string executionId) =>
            (Request == null) ? default : $"{Request.Scheme}://{Request.Host}/execution/{executionId}/status";

        private string PutExecutionStatusUrl(string executionId) =>
            (Request == null) ? default : $"{Request.Scheme}://{Request.Host}/execution/{executionId}/status";

        private Core.Models.Execution ApplyExecutionUpdate(UpdateExecutionStatusApiModel updateApiModel, Core.Models.Execution execution)
        {
            execution.LastUpdatedDateTimeUtc = (updateApiModel.LastUpdatedDateTimeUtc ?? DateTime.UtcNow);
            execution.PercentComplete = updateApiModel.PercentComplete;
            execution.ProvidedOutputObjects.AddRange(updateApiModel.ProvidedOutputObjects.Except(execution.ProvidedOutputObjects));
            execution.ResultData = (updateApiModel.ResultData ?? execution.ResultData);
            execution.StatusMessage = updateApiModel.StatusMessage;
            execution.ExecutionTimeoutDateTimeUtc = updateApiModel.ExecutionTimeoutDateTimeUtc.GetValueOrDefault();

            execution.ValidationErrors.AddRange(
                updateApiModel.ValidationErrors
                    .Where(ve => execution.ValidationErrors.None(eve => (ve.ErrorId == eve.ErrorId)))
                    .Select(ve => ve.ToCoreModel()));

            return execution;
        }

        private Core.Models.Execution ApplyExecutionContext(Core.Models.ExecutionContext execContext, Core.Models.Execution execution)
        {
            execution.LastUpdatedDateTimeUtc = DateTime.UtcNow;
            execution.PercentComplete = execContext.PercentComplete;

            if (execContext.ProvidedOutputObjects != null)
            {
                execution.ProvidedInputObjects.AddRange(
                    execContext.ProvidedInputObjects.Except(execution.ProvidedInputObjects));
            }

            if (execContext.ProvidedOutputObjects != null)
            {
                execution.ProvidedOutputObjects.AddRange(
                    execContext.ProvidedOutputObjects.Except(execution.ProvidedOutputObjects));
            }

            execution.ResultData = (execContext.ResultData ?? execution.ResultData);
            execution.Status = execContext.Status;
            execution.StatusMessage = execContext.StatusMessage;
            execution.ExecutionTimeoutDateTimeUtc = execContext.ExecutionTimeoutDateTimeUtc.GetValueOrDefault();

            if (execContext.ValidationErrors != null)
            {
                execution.ValidationErrors.AddRange(execContext.ValidationErrors);
            }

            return execution;
        }

        private async Task<ExecutionUpdateApiModel> ToExecutionUpdateApiModelAsync(Core.Models.Execution execution, string signatureRsaKeyXml = null)
        {
            var apiModel = new ExecutionUpdateApiModel
            {
                ExecutionId = execution.ExecutionId,
                ExecutionStatus = execution.Status.ToString(),
                ExpirationDateTimeUtc = execution.ExecutionTimeoutDateTimeUtc,
                LastUpdatedDateTimeUtc = execution.LastUpdatedDateTimeUtc,
                PercentComplete = execution.PercentComplete,
                ResultData = execution.ResultData,
                StatusMessage = execution.StatusMessage,
                GetExecutionStatusUrl = GetExecutionStatusUrl(execution.ExecutionId),
                PutExecutionStatusUrl = PutExecutionStatusUrl(execution.ExecutionId),
                ExecutorProperties = execution.ExecutorProperties
            };

            if (execution.Status == ExecutionStatus.PendingInputObjects)
            {
                apiModel.InputObjects = await extensionObjectApiModelService.CreateInputObjectDictionaryAsync(
                    execution.InputObjects.Where(io => execution.ProvidedInputObjects.Contains(io.Name) == false),
                    execution);
            }

            apiModel.OutputObjects = await extensionObjectApiModelService.CreateOutputObjectDictionaryAsync(
                execution.OutputObjects.Where(oo => execution.ProvidedOutputObjects.Contains(oo.Name)),
                execution);

            if (execution.ValidationErrors?.Any() == true)
            {
                apiModel.ValidationErrors = execution.ValidationErrors.Select(ve => ve.ToApiModel()).ToList();
            }

            return apiModel;
        }

        private async Task<Core.Models.Execution> ToExecutionAsync(ExecutionRequestContext<ExecutionRequestApiModel> erContext) =>
            new Core.Models.Execution
            {
                CreatedDateTimeUtc = DateTime.UtcNow,
                ExecutionId = erContext.ExtensionVersion.CreateNewExecutionId(userContext.Executor.TenantId),
                ExecutionModelName = erContext.ExecutionProfile.ExecutionModelName,
                ExecutionProfileName = erContext.ExecutionProfile.ProfileName,
                Executor = userContext.Executor,
                ExtensionId = erContext.Extension.ExtensionId,
                ExtensionVersionId = erContext.ExtensionVersion.ExtensionVersionId,
                LastUpdatedDateTimeUtc = DateTime.UtcNow,
                ObjectProviderName = erContext.ExecutionProfile.ObjectProviderName,
                Priority = Enum.Parse<ExecutionPriority>(erContext.OriginalRequest.Priority),
                StatusUpdateKey = Guid.NewGuid().ToString(),
                ValidateOnly = erContext.OriginalRequest.ValidateOnly,
                InputObjects = erContext.ExtensionVersion.InputObjects,
                OutputObjects = erContext.ExtensionVersion.OutputObjects,
                RequestData = erContext.OriginalRequest.RequestData,
                Mode = erContext.ExecutionProfile.ExecutionMode,
                SignatureRsaKeyXml = await extensionRsaKeyProvider.GetExtensionRsaKeyXmlAsync(erContext.Extension),
                ExecutorProperties = erContext.OriginalRequest.ExecutorProperties
            };

        private async Task<ExecutionRequest> ToExecutionRequestAsync(IExecutionRequestContext erContext) =>
            new ExecutionRequest
            {
                CreatedDateTimeUtc = erContext.Execution.CreatedDateTimeUtc,
                ExecutionId = erContext.Execution.ExecutionId,
                ExecutionParameters = erContext.Execution.RequestData,
                ExecutionProfileName = erContext.Execution.ExecutionProfileName,
                ExecutionTimeoutDuration = erContext.ExtensionVersion.ExecutionExpirationPeriod,
                Executor = erContext.Execution.Executor,
                ExtensionId = erContext.Execution.ExtensionId,
                ExtensionSettings = await extensionSettingsBuilder.BuildExtensionSettingsAsync(erContext),
                ExtensionVersionId = erContext.Execution.ExtensionVersionId,
                InputObjects = erContext.ExtensionVersion.InputObjects.ToDictionary(io => io.Name),
                LastUpdatedDateTimeUtc = erContext.Execution.LastUpdatedDateTimeUtc,
                ObjectProviderName = erContext.Execution.ObjectProviderName,
                IsValidationSupported = erContext.ExtensionVersion.SupportsValidation,
                OutputObjects = erContext.ExtensionVersion.OutputObjects.ToDictionary(oo => oo.Name),
                Priority = erContext.Execution.Priority,
                StatusUpdateKey = erContext.Execution.StatusUpdateKey,
                SupportedServices = erContext.ExtensionVersion.SupportedServices,
                ValidateOnly = erContext.Execution.ValidateOnly,
                GetExecutionStatusUrl = GetExecutionStatusUrl(erContext.Execution.ExecutionId),
                UpdateExecutionStatusUrl = PutExecutionStatusUrl(erContext.Execution.ExecutionId),
                ExecutionModelName = erContext.ExecutionProfile.ExecutionModelName,
                ProvidedInputObjects = erContext.Execution.ProvidedInputObjects,
                SignatureRsaKeyXml = erContext.Execution.SignatureRsaKeyXml,
                ExecutorProperties = erContext.Execution.ExecutorProperties
            };
    }
}
