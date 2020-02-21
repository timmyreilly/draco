// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models.Enumerations;
using Draco.Core.Models.Extensions;
using Draco.Core.Models.Interfaces;
using Draco.Execution.Api.Constants;
using Draco.Execution.Api.Interfaces;
using Draco.Execution.Api.Models;
using System;
using System.Threading.Tasks;

namespace Draco.Execution.Api.Services
{
    public class ExecutionRequestContextBuilder : IExecutionRequestContextBuilder
    {
        private readonly IExecutionRequestContextValidator erContextValidator;
        private readonly IExtensionRepository extensionRepository;

        public ExecutionRequestContextBuilder(IExecutionRequestContextValidator erContextValidator, IExtensionRepository extensionRepository)
        {
            this.erContextValidator = erContextValidator;
            this.extensionRepository = extensionRepository;
        }

        public async Task<ExecutionRequestContext<ExecutionRequestApiModel>> BuildExecutionRequestContextAsync(ExecutionRequestApiModel apiExecRequest)
        {
            var erContext = new ExecutionRequestContext<ExecutionRequestApiModel>(apiExecRequest);

            if (string.IsNullOrEmpty(apiExecRequest.ExtensionId))
            {
                erContext.ValidationErrors.Add($"[{ErrorCodes.ExtensionIdNotProvided}]: [extensionId] is required.");
            }

            if (string.IsNullOrEmpty(apiExecRequest.ExtensionVersionId))
            {
                erContext.ValidationErrors.Add($"[{ErrorCodes.ExtensionVersionIdNotProvided}]: [extensionVersionId] is required.");
            }

            if (Enum.TryParse<ExecutionPriority>(apiExecRequest.Priority, out var execPriority) == false)
            {
                erContext.ValidationErrors.Add($"[{ErrorCodes.InvalidPriority}]: [{apiExecRequest.Priority}] is not a valid [priority]; " +
                                               $"valid priorities are [{ExecutionPriority.Low}], [{ExecutionPriority.Normal}], " +
                                               $"and [{ExecutionPriority.High}]; if no [priority] is explicitly provided, " +
                                               $"[{ExecutionPriority.Normal}] is automatically selected.");
            }

            if (string.IsNullOrEmpty(apiExecRequest.ExtensionId) == false)
            {
                erContext.Extension = await extensionRepository.GetExtensionAsync(apiExecRequest.ExtensionId);

                if (erContext.Extension == null)
                {
                    erContext.ValidationErrors.Add($"[{ErrorCodes.ExtensionNotFound}]: Extension [{apiExecRequest.ExtensionId}] not found.");
                }
                else if (erContext.Extension.IsActive == false)
                {
                    erContext.ValidationErrors.Add($"[{ErrorCodes.ExtensionDisabled}]: Extension [{apiExecRequest.ExtensionId}] is currently unavailable.");
                }
                else if (string.IsNullOrEmpty(apiExecRequest.ExtensionVersionId) == false)
                {
                    erContext.ExtensionVersion = erContext.Extension.GetExtensionVersion(apiExecRequest.ExtensionVersionId);

                    if (erContext.ExtensionVersion == null)
                    {
                        erContext.ValidationErrors.Add($"[{ErrorCodes.ExtensionVersionNotFound}]: Extension [{apiExecRequest.ExtensionId}] version " +
                                                       $"[{apiExecRequest.ExtensionVersionId}] not found.");
                    }
                    else if (erContext.ExtensionVersion.IsActive == false)
                    {
                        erContext.ValidationErrors.Add($"[{ErrorCodes.ExtensionVersionDisabled}]: Extension [{apiExecRequest.ExtensionId}] version " +
                                                       $"[{apiExecRequest.ExtensionVersionId}] is currently unavailable.");
                    }
                    else
                    {
                        if (apiExecRequest.ValidateOnly && erContext.ExtensionVersion.SupportsValidation == false)
                        {
                            erContext.ValidationErrors.Add($"[{ErrorCodes.ExtensionVersionDoesNotSupportValidation}]: " +
                                                           $"Extension [{apiExecRequest.ExtensionId}] version " +
                                                           $"[{apiExecRequest.ExtensionVersionId}] does not support validation.");
                        }

                        erContext.ExecutionProfile = erContext.ExtensionVersion.GetExecutionProfile(apiExecRequest.ProfileName);

                        if (erContext.ExecutionProfile == null)
                        {
                            erContext.ValidationErrors.Add($"[{ErrorCodes.ExecutionProfileNotFound}]: " +
                                                           $"Execution profile [{apiExecRequest.ProfileName}] not found.");
                        }
                        else
                        {
                            if (erContext.ExecutionProfile.SupportedPriorities.HasFlag(execPriority) == false)
                            {
                                erContext.ValidationErrors.Add($"[{ErrorCodes.PriorityNotSupported}]: " +
                                                               $"Execution profile [{apiExecRequest.ProfileName}] does not support " +
                                                               $"[{execPriority}]-priority execution.");
                            }

                            await erContextValidator.ValidateExecutionRequestContextAsync(erContext);
                        }
                    }
                }
            }

            return erContext;
        }
    }
}
