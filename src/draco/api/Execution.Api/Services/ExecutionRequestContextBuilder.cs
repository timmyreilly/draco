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
    /// <summary>
    /// This class pulls together all the information needed to process an API execution request.
    /// In the process, this class also validates the execution request against the appropriate extension, extension version, and execution profile.
    /// </summary>
    public class ExecutionRequestContextBuilder : IExecutionRequestContextBuilder
    {
        private readonly IExtensionRepository extensionRepository;

        public ExecutionRequestContextBuilder(IExtensionRepository extensionRepository)
        {
            this.extensionRepository = extensionRepository;
        }

        public async Task<ExecutionRequestContext<ExecutionRequestApiModel>> BuildExecutionRequestContextAsync(ExecutionRequestApiModel apiExecRequest)
        {
            // Create a new execution request context...

            var erContext = new ExecutionRequestContext<ExecutionRequestApiModel>(apiExecRequest);

            // Did they provide an extension ID?

            if (string.IsNullOrEmpty(apiExecRequest.ExtensionId))
            {
                erContext.ValidationErrors.Add($"[{ErrorCodes.ExtensionIdNotProvided}]: [extensionId] is required.");
            }

            // Did they provide an extension version ID?

            if (string.IsNullOrEmpty(apiExecRequest.ExtensionVersionId))
            {
                erContext.ValidationErrors.Add($"[{ErrorCodes.ExtensionVersionIdNotProvided}]: [extensionVersionId] is required.");
            }

            // Is the execution priority that they provided valid?

            if (Enum.TryParse<ExecutionPriority>(apiExecRequest.Priority, out var execPriority) == false)
            {
                erContext.ValidationErrors.Add($"[{ErrorCodes.InvalidPriority}]: [{apiExecRequest.Priority}] is not a valid [priority]; " +
                                               $"valid priorities are [{ExecutionPriority.Low}], [{ExecutionPriority.Normal}], " +
                                               $"and [{ExecutionPriority.High}]; if no [priority] is explicitly provided, " +
                                               $"[{ExecutionPriority.Normal}] is automatically selected.");
            }

            if (string.IsNullOrEmpty(apiExecRequest.ExtensionId) == false)
            {
                // Get the extension information...

                erContext.Extension = await extensionRepository.GetExtensionAsync(apiExecRequest.ExtensionId);

                // Were we able to find the extension and is it active?

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
                    // Get the extension version information...

                    erContext.ExtensionVersion = erContext.Extension.GetExtensionVersion(apiExecRequest.ExtensionVersionId);

                    // Were we able to find the extension version and is it active?

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
                        // If the user has only requested validation (not execution), does the extension version support validation?

                        if (apiExecRequest.ValidateOnly && erContext.ExtensionVersion.SupportsValidation == false)
                        {
                            erContext.ValidationErrors.Add($"[{ErrorCodes.ExtensionVersionDoesNotSupportValidation}]: " +
                                                           $"Extension [{apiExecRequest.ExtensionId}] version " +
                                                           $"[{apiExecRequest.ExtensionVersionId}] does not support validation.");
                        }

                        // Get the execution profile information...

                        erContext.ExecutionProfile = erContext.ExtensionVersion.GetExecutionProfile(apiExecRequest.ProfileName);

                        // Were we able to find the execution profile information?

                        if (erContext.ExecutionProfile == null)
                        {
                            erContext.ValidationErrors.Add($"[{ErrorCodes.ExecutionProfileNotFound}]: " +
                                                           $"Execution profile [{apiExecRequest.ProfileName}] not found.");
                        }
                        else
                        {
                            // And, finally, is the specified priority supported by the execution profile?

                            if (erContext.ExecutionProfile.SupportedPriorities.HasFlag(execPriority) == false)
                            {
                                erContext.ValidationErrors.Add($"[{ErrorCodes.PriorityNotSupported}]: " +
                                                               $"Execution profile [{apiExecRequest.ProfileName}] does not support " +
                                                               $"[{execPriority}]-priority execution.");
                            }
                        }
                    }
                }
            }

            return erContext;
        }
    }
}
