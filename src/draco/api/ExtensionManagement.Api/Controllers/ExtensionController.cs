// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Extensions;
using Draco.Core.Models;
using Draco.Core.Models.Enumerations;
using Draco.Core.Models.Extensions;
using Draco.Core.Models.Interfaces;
using Draco.ExtensionManagement.Api.Extensions;
using Draco.ExtensionManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Draco.ExtensionManagement.Api.Controllers
{
    [Route("extensions")]
    [Produces("application/json")]
    [ApiController]
    public class ExtensionController : ControllerBase
    {
        private readonly IExtensionRepository extensionRepository;

        public ExtensionController(IExtensionRepository extensionRepository)
        {
            this.extensionRepository = extensionRepository;
        }

        /// <summary>
        /// Creates a new extension
        /// </summary>
        /// <param name="extensionApiModel">The new extension definition</param>
        /// <returns></returns>
        /// <response code="201">Extension created.</response>
        /// <response code="400">Details included in response.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiModelContainer<ExtensionApiModel>), 201)]
        public async Task<IActionResult> CreateExtensionAsync([Required, FromBody] ExtensionApiModel extensionApiModel)
        {
            // Validate the provided extension API model.

            var errors = Validate(extensionApiModel);

            // Respond with [400 Bad Request] + detailed error description if there are any errors...

            if (errors.Any())
            {
                return BadRequest(string.Join(' ', errors));
            }

            // Convert the API model to a core extension model...

            var extension = extensionApiModel.ToCoreModel();

            // Save the extension...

            await extensionRepository.SaveExtensionAsync(extension);

            // Let the user know that the extension has been created...

            return new CreatedResult(GetGetExtensionUrl(extension.ExtensionId), ToApiModelContainer(extension));
        }
        
        /// <summary>
        /// Gets the specified extension
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <returns></returns>
        /// <response code="200">Extension returned.</response>
        /// <response code="404">Extension not found.</response>
        [HttpGet("{extensionId}")]
        [ProducesResponseType(typeof(ApiModelContainer<ExtensionApiModel>), 200)]
        public async Task<IActionResult> GetExtensionAsync([Required] string extensionId)
        {
            // Try to get the extension...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we couldn't find it, respond with [404 Not Found]...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            // Otherwise, return the extension...

            return Ok(ToApiModelContainer(extension));
        }

        /// <summary>
        /// Deletes the specified extension
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpDelete("{extensionId}")]
        public async Task<IActionResult> DeleteExtensionAsync([Required] string extensionId)
        {
            // Try to get the extension...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we find it, don't actually delete it, but deactivate it...

            if (extension?.IsActive == true)
            {
                extension.IsActive = false;

                await extensionRepository.SaveExtensionAsync(extension);
            }

            // Regardless of whether or not we found it, respond with [200 OK]...

            return Ok();
        }

        /// <summary>
        /// Creates a new extension version
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionApiModel">The new extension version definition</param>
        /// <returns></returns>
        /// <response code="201">Extension version created.</response>
        /// <response code="400">Details included in response.</response>
        /// <response code="404">Extension not found.</response>
        /// <response code="409">Extension version already exists.</response>
        [HttpPost("{extensionId}/versions")]
        [ProducesResponseType(typeof(ApiModelContainer<ExtensionVersionApiModel>), 201)]
        public async Task<IActionResult> CreateExtensionVersionAsync([Required] string extensionId, 
                                                                     [Required, FromBody] ExtensionVersionApiModel exVersionApiModel)
        {
            // Validate the extension version API model...

            var errors = Validate(exVersionApiModel);

            // If any validation errors were encoutered, respond with a [400 Bad Request] + a detailed error description...

            if (errors.Any())
            {
                return BadRequest(string.Join(' ', errors));
            }

            // Parse the provided version string (<major>.[minor].[patch])...

            var version = new Version(exVersionApiModel.Version);

            // Try to get the extension to which we want to add a new version...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we couldn't find the extension, respond with [404 Not Found]...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            // Check to see if the specified version already exists...

            var exVersion = extension.ExtensionVersions.SingleOrDefault(ev => Version.Parse(ev.Version) == version);

            if (exVersion != null)
            {
                // If it already exists, we can't add it again. Respond with a [409 Conflict]...
                // TODO: Implement [PUT] action for updating existing extension version.

                return new ConflictObjectResult($"Extension [{extensionId}] version [{version}] already exists at " +
                                                $"[{GetGetExtensionVersionUrl(extensionId, exVersion.ExtensionVersionId)}].");
            }

            // Now that we've checked everything, we're good to add the new extension version.
            // Convert the extension version API model to a core model and save it...

            exVersion = exVersionApiModel.ToCoreModel(extensionId);

            await extensionRepository.SaveExtensionVersionAsync(exVersion);

            // Let the client know that the new extension version has been created...

            return new CreatedResult(
                GetGetExtensionVersionUrl(extensionId, exVersion.ExtensionVersionId),
                ToApiModelContainer(exVersion, extensionId));
        }

        /// <summary>
        /// Deletes the specified extension version
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpDelete("{extensionId}/versions/{exVersionId}")]
        public async Task<IActionResult> DeleteExtensionVersionAsync([Required] string extensionId, [Required] string exVersionId)
        {
            // Try to find the extension and extension version...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);
            var exVersion = extension?.GetExtensionVersion(exVersionId);

            // If we found the extension version, don't delete it, but deactivate it.

            if (exVersion?.IsActive == true)
            {
                exVersion.IsActive = false;

                // Save the extension version...

                await extensionRepository.SaveExtensionVersionAsync(exVersion);
            }

            // Regardless of whether or not we found the extension version, respond with [200 OK]...

            return Ok();
        }

        /// <summary>
        /// Gets all versions of the specified extension
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <returns></returns>
        /// <response code="200">Extension version(s) returned.</response>
        /// <response code="404">Extension not found.</response>
        [HttpGet("{extensionId}/versions")]
        [ProducesResponseType(typeof(IEnumerable<ApiModelContainer<ExtensionVersionApiModel>>), 200)]
        public async Task<IActionResult> GetExtensionVersionsAsync([Required] string extensionId)
        {
            // Try to find the specified extension...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we couldn't find it, respond with [404 Not Found]...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            // Respond with [200 OK] + a list of extension versions...
            // TODO: In the future, we may want to consider adding paging here...

            return Ok(extension.ExtensionVersions.Select(ev => ToApiModelContainer(ev, extensionId)));
        }

        /// <summary>
        /// Gets the specified extension version
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <returns></returns>
        /// <response code="200">Extension version returned.</response>
        /// <response code="404">Extension or extension version not found.</response>
        [HttpGet("{extensionId}/versions/{exVersionId}")]
        [ProducesResponseType(typeof(ApiModelContainer<ExtensionVersionApiModel>), 200)]
        public async Task<IActionResult> GetExtensionVersionAsync([Required] string extensionId, [Required] string exVersionId)
        {
            // Try to get the specified extension...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we can't find the extension, respond with [404 Not Found]...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            // Try to find the specified extension version...

            var exVersion = extension.GetExtensionVersion(exVersionId);

            // If we can't find the extension version, respond with [404 Not Found]...

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            // Respond with [200 OK] + the extension version...

            return Ok(ToApiModelContainer(exVersion, extensionId));
        }

        /// <summary>
        /// Creates a new extension version input object
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <param name="inputObjectApiModel">The input object definition</param>
        /// <returns></returns>
        /// <response code="201">Input object created.</response>
        /// <response code="400">Details included in response.</response>
        /// <response code="404">Extension or extension version not found.</response>
        /// <response code="409">Input object already exists.</response>
        [HttpPost("{extensionId}/versions/{exVersionId}/objects/input")]
        [ProducesResponseType(typeof(ApiModelContainer<InputObjectApiModel>), 201)]
        public async Task<IActionResult> CreateInputObjectAsync([Required] string extensionId, [Required] string exVersionId, 
                                                                [Required, FromBody] InputObjectApiModel inputObjectApiModel)
        {
            // Validate the input object API model...

            var errors = Validate(inputObjectApiModel);

            // If there were any validation errors, respond with [400 Bad Request] + detailed error description...

            if (errors.Any())
            {
                return BadRequest(string.Join(' ', errors));
            }

            // Try to the find target extension...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we can't find the extesnion, respond with [404 Not Found]...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            // Try to find the target extension version...

            var exVersion = extension.GetExtensionVersion(exVersionId);

            // If we can't find the extension version, respond with [404 Not Found]...

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            // Check to see if the input object already exists...

            var inputObjectName = inputObjectApiModel.Name.ToLower();
            var inputObject = exVersion.InputObjects.SingleOrDefault(io => (io.Name == inputObjectName));

            if (inputObject != null)
            {
                // If the input object already exists, respond with [409 Conflict]...
                // TODO: Implement PUT action for updating existing input objects.

                return new ConflictObjectResult($"Extension [{extensionId}] version [{exVersionId}] input object [{inputObjectApiModel.Name}] " +
                                                $"already exists at [{GetGetInputObjectUrl(extensionId, exVersionId, inputObjectName)}].");
            }

            // Validation is done and we're ready to add the input object.
            // Convert the input object API model to its core model counterpart and save it to the extension version...

            inputObject = inputObjectApiModel.ToCoreModel();

            exVersion.InputObjects.Add(inputObject);

            await extensionRepository.SaveExtensionVersionAsync(exVersion);

            // Let the client know that the input object has been created...

            return new CreatedResult(
                GetGetInputObjectUrl(extensionId, exVersionId, inputObjectName),
                ToApiModelContainer(inputObject, extensionId, exVersionId));
        }

        /// <summary>
        /// Deletes the specified input object
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <param name="objectName">The input object name</param>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpDelete("{extensionId}/versions/{exVersionId}/objects/input/{objectName}")]
        public async Task<IActionResult> DeleteInputObjectAsync([Required] string extensionId, [Required] string exVersionId, [Required] string objectName)
        {
            // Try to find the extension, extension version, and input object...

            objectName = WebUtility.UrlDecode(objectName);

            var extension = await extensionRepository.GetExtensionAsync(extensionId);
            var exVersion = extension?.GetExtensionVersion(exVersionId);
            var inputObject = exVersion?.GetInputObject(objectName.ToLower());

            // If we find the input object, remove it, and save the updated extension version...

            if (inputObject != null)
            {
                exVersion.InputObjects.Remove(inputObject);

                await extensionRepository.SaveExtensionVersionAsync(exVersion);
            }

            // Regardless of whether or not we find the input object, respond with [200 OK]...

            return Ok();
        }

        /// <summary>
        /// Gets all input objects associated with the specified extension version
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <returns></returns>
        /// <response code="200">Input object(s) returned.</response>
        /// <response code="404">Extension or extension version not found.</response>
        [HttpGet("{extensionId}/versions/{exVersionId}/objects/input")]
        [ProducesResponseType(typeof(IEnumerable<ApiModelContainer<InputObjectApiModel>>), 200)]
        public async Task<IActionResult> GetInputObjectsAsync([Required] string extensionId, [Required] string exVersionId)
        {
            // Try to find the specified extension...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we can't find the extension, respond with [404 Not Found]...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            // Try to find the speciifed extension version...

            var exVersion = extension.GetExtensionVersion(exVersionId);

            // If we can't find the extension version, respond with [404 Not Found]...

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            // Respond with [200 OK] + a list of all input objects...
            // TODO: May want to implement paging here in the future.

            return Ok(exVersion.InputObjects.Select(io => ToApiModelContainer(io, extensionId, exVersionId)));
        }

        /// <summary>
        /// Gets the specified input object
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <param name="objectName">The input object name</param>
        /// <returns></returns>
        /// <response code="200">Input object returned.</response>
        /// <response code="404">Extension, extension version, or input object not found.</response>
        [HttpGet("{extensionId}/versions/{exVersionId}/objects/input/{objectName}")]
        [ProducesResponseType(typeof(ApiModelContainer<InputObjectApiModel>), 200)]
        public async Task<IActionResult> GetInputObjectAsync([Required] string extensionId, [Required] string exVersionId, [Required] string objectName)
        {
            objectName = WebUtility.UrlDecode(objectName);

            // Try to find the specified extension...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we can't find the extension, respond with [404 Not Found]...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            // Try to find the specified extension version...

            var exVersion = extension.GetExtensionVersion(exVersionId);

            // If we can't find the extension version, respond with [404 Not Found]...

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            // Try to find the specified input object... 

            var inputObject = exVersion.InputObjects.SingleOrDefault(io => io.Name == objectName.ToLower());

            // If we can't find the input object, respond with [404 Not Found]...

            if (inputObject == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] input object [{objectName}] not found.");
            }

            // Finally, if we did find the input object, respond with [200 OK] + the input object definition...

            return Ok(ToApiModelContainer(inputObject, extensionId, exVersionId));
        }

        /// <summary>
        /// Creates a new extension version output object
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <param name="outputObjectApiModel">The output object definition</param>
        /// <returns></returns>
        /// <response code="201">Output object created.</response>
        /// <response code="400">Details included in response.</response>
        /// <response code="404">Extension or extension version not found.</response>
        /// <response code="409">Output object already exists.</response>
        [HttpPost("{extensionId}/versions/{exVersionId}/objects/output")]
        [ProducesResponseType(typeof(ApiModelContainer<OutputObjectApiModel>), 201)]
        public async Task<IActionResult> CreateOutputObjectAsync([Required] string extensionId, [Required] string exVersionId, 
                                                                 [Required, FromBody] OutputObjectApiModel outputObjectApiModel)
        {
            // Validate the output object API model...

            var errors = Validate(outputObjectApiModel);

            // If there were any validation errors, respond with [400 Bad Request] + detailed error description...

            if (errors.Any())
            {
                return BadRequest(string.Join(' ', errors));
            }

            // Try to find the specified extension...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we can't find the extension, respond with [404 Not Found]...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            // Try to find the specified extension version...

            var exVersion = extension.GetExtensionVersion(exVersionId);

            // If we can't find the extension version, respond with [404 Not Found]...

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            // Check to see if the output object already exists...

            var outputObjectName = outputObjectApiModel.Name.ToLower();
            var outputObject = exVersion.OutputObjects.SingleOrDefault(oo => oo.Name == outputObjectName);

            if (outputObject != null)
            {
                // If it does already exist, respond with [409 Conflict]...
                // TODO: Implement output object PUT action for updates.

                return new ConflictObjectResult($"Extension [{extensionId}] version [{exVersionId}] output object [{outputObjectApiModel.Name}] " +
                                                $"already exists at [{GetGetOutputObjectUrl(extensionId, exVersionId, outputObjectName)}].");
            }

            // Convert the output object API model to its core model counterpart, add it to the extension version, and save the extension version...

            outputObject = outputObjectApiModel.ToCoreModel();

            exVersion.OutputObjects.Add(outputObject);

            await extensionRepository.SaveExtensionVersionAsync(exVersion);

            // Let the client know that we've created the new output object...

            return new CreatedResult(
                GetGetOutputObjectUrl(extensionId, exVersionId, outputObjectName),
                ToApiModelContainer(outputObject, extensionId, exVersionId));
        }

        /// <summary>
        /// Deletes the specified output object
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <param name="objectName">The output object name</param>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpDelete("{extensionId}/versions/{exVersionId}/objects/output/{objectName}")]
        public async Task<IActionResult> DeleteOutputObjectAsync([Required] string extensionId, [Required] string exVersionId, [Required] string objectName)
        {
            objectName = WebUtility.UrlDecode(objectName);

            // Check to see whether the specified extension, extension version, and output object exist...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);
            var exVersion = extension?.GetExtensionVersion(exVersionId);
            var outputObject = exVersion?.GetOutputObject(objectName.ToLower());

            // If the output object does exist, remove it from and save the extension version...

            if (outputObject != null)
            {
                exVersion.OutputObjects.Remove(outputObject);

                await extensionRepository.SaveExtensionVersionAsync(exVersion);
            }

            // Regardless of whether or not we found the output object, respond with [200 OK]...

            return Ok();
        }

        /// <summary>
        /// Gets all output objects associated with the specified extension version
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <returns></returns>
        /// <response code="200">Output object(s) returned.</response>
        /// <response code="404">Extension or extension version not found.</response>
        [HttpGet("{extensionId}/versions/{exVersionId}/objects/output")]
        [ProducesResponseType(typeof(IEnumerable<ApiModelContainer<OutputObjectApiModel>>), 200)]
        public async Task<IActionResult> GetOutputObjectsAsync([Required] string extensionId, [Required] string exVersionId)
        {
            // Try to find the specified extension...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we can't find the extension, respond with [404 Not Found]...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            // Try to find the specified extension version...

            var exVersion = extension.GetExtensionVersion(exVersionId);

            // If we can't find the extension version, respond with [404 Not Found]...

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            // Respond with [200 OK] + a list of the specified extension version's output objects...
            // TODO: May implement paging at some point in the future.

            return Ok(exVersion.OutputObjects.Select(oo => ToApiModelContainer(oo, extensionId, exVersionId)));
        }

        /// <summary>
        /// Gets the specified output object
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <param name="objectName">The output object name</param>
        /// <returns></returns>
        /// <response code="200">Output object returned.</response>
        /// <response code="404">Extension, extension version, or output object not found.</response>
        [HttpGet("{extensionId}/versions/{exVersionId}/objects/output/{objectName}")]
        [ProducesResponseType(typeof(ApiModelContainer<OutputObjectApiModel>), 200)]
        public async Task<IActionResult> GetOutputObjectAsync([Required] string extensionId, [Required] string exVersionId, [Required] string objectName)
        {
            objectName = WebUtility.UrlDecode(objectName);

            // Try to find the specified extension...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we couldn't find the extension, respond with [404 Not Found]...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            // Try to find the specified extension version...

            var exVersion = extension.GetExtensionVersion(exVersionId);

            // If we couldn't find the extension version, respond with [404 Not Found]...

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            // Try to find the specified output object...

            var outputObject = exVersion.OutputObjects.SingleOrDefault(oo => oo.Name == objectName.ToLower());

            // If we couldn't find the output object, respond with [404 Not Found]...

            if (outputObject == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] output object [{objectName}] not found.");
            }

            // If we found the output object, respond with [200 OK] + the output object definition...

            return Ok(ToApiModelContainer(outputObject, extensionId, exVersionId));
        }

        /// <summary>
        /// Creates a new execution profile
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <param name="execProfileApiModel">The execution profile definition</param>
        /// <returns></returns>
        /// <response code="201">Execution profile created.</response>
        /// <response code="400">Details included in response.</response>
        /// <response code="404">Extension or extension version not found.</response>
        /// <response code="409">Execution profile already exists.</response>
        [HttpPost("{extensionId}/versions/{exVersionId}/profiles")]
        [ProducesResponseType(typeof(ApiModelContainer<ExecutionProfileApiModel>), 201)]
        public async Task<IActionResult> CreateExecutionProfileAsync([Required] string extensionId, [Required] string exVersionId, 
                                                                     [Required, FromBody] ExecutionProfileApiModel execProfileApiModel)
        {
            // Validate the execution profile API model...

            var errors = Validate(execProfileApiModel);

            // If there were any validation errors, respond with [400 Bad Request] + detailed error description...

            if (errors.Any())
            {
                return BadRequest(string.Join(' ', errors));
            }

            // Try to find the specified extension...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we couldn't find the extension, respond with [404 Not Found]...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            // Try to find the specified extension version...

            var exVersion = extension.GetExtensionVersion(exVersionId);

            // If we couldn't find the extension version, respond with [404 Not Found]...

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            // Check to make sure that the execution profile doesn't already exist...

            var execProfileName = execProfileApiModel.Name.ToLower();
            var execProfile = exVersion.ExecutionProfiles.SingleOrDefault(p => p.ProfileName == execProfileName);

            if (execProfile != null)
            {
                // If it does already exist, respond with [409 Conflict]...

                return new ConflictObjectResult($"Extension [{extensionId}] version [{exVersionId}] execution profile [{execProfileApiModel.Name}] " +
                                                $"already exists at [{GetGetExecutionProfileUrl(extensionId, exVersionId, execProfileName)}].");
            }

            // Convert the execution profile API model to its core model counterpart, add it to the extension version,
            // and save the extension version.

            execProfile = execProfileApiModel.ToCoreModel();

            exVersion.ExecutionProfiles.Add(execProfile);

            await extensionRepository.SaveExtensionVersionAsync(exVersion);

            // Let the client know that the execution profile has been created...

            return new CreatedResult(
                GetGetExecutionProfileUrl(extensionId, exVersionId, execProfileName),
                ToApiModelContainer(execProfile, extensionId, exVersionId));
        }

        /// <summary>
        /// Deletes the specified execution profile
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <param name="profileName">The execution profile name</param>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpDelete("{extensionId}/versions/{exVersionId}/profiles/{profileName}")]
        public async Task<IActionResult> DeleteExecutionProfileAsync([Required] string extensionId, [Required] string exVersionId, [Required] string profileName)
        {
            profileName = WebUtility.UrlDecode(profileName);

            // Try to find the specified extension, extension version, and execution profile...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);
            var exVersion = extension?.GetExtensionVersion(exVersionId);
            var exProfile = exVersion?.GetExecutionProfile(profileName);

            // If we find the execution profile, remove it from and save the extension version...

            if (exProfile != null)
            {
                exVersion.ExecutionProfiles.Remove(exProfile);

                await extensionRepository.SaveExtensionVersionAsync(exVersion);
            }

            // Regardless of whether or not we found the execution profile, respond with [200 OK]...

            return Ok();
        }

        /// <summary>
        /// Gets all execution profile(s) associated with the specified extension version
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <returns></returns>
        /// <response code="200">Execution profile(s) returned.</response>
        /// <response code="404">Extension or extension version not found.</response>
        [HttpGet("{extensionId}/versions/{exVersionId}/profiles")]
        [ProducesResponseType(typeof(IEnumerable<ApiModelContainer<ExecutionProfileApiModel>>), 200)]
        public async Task<IActionResult> GetExecutionProfilesAsync([Required] string extensionId, [Required] string exVersionId)
        {
            // Try to find the specified extension...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we can't find the extension, respond with [404 Not Found]...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            // Try to find the specified extension version...

            var exVersion = extension.GetExtensionVersion(exVersionId);

            // If we can't find the extension version, respond with [404 Not Found]...

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            // Once we find the extension version, respond with [200 OK] + a list of all associated execution profiles...

            return Ok(exVersion.ExecutionProfiles.Select(ep => ToApiModelContainer(ep, extensionId, exVersionId)));
        }

        /// <summary>
        /// Gets the specified execution profile
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <param name="profileName">The execution profile name</param>
        /// <returns></returns>
        /// <response code="200">Execution profile returned.</response>
        /// <response code="404">Extension, extension version, or execution profile not found.</response>
        [HttpGet("{extensionId}/versions/{exVersionId}/profiles/{profileName}")]
        [ProducesResponseType(typeof(ApiModelContainer<ExecutionProfileApiModel>), 200)]
        public async Task<IActionResult> GetExecutionProfileAsync([Required] string extensionId, [Required] string exVersionId, [Required] string profileName)
        {
            profileName = WebUtility.UrlDecode(profileName);

            // Try to find the specified extension...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we can't find the extension, respond with [404 Not Found]...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            // Try to find the specified extension version...

            var exVersion = extension.GetExtensionVersion(exVersionId);

            // If we can't find the extension version, respond with [404 Not Found]...

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            // Try to find the specified execution profile...

            var execProfile = exVersion.GetExecutionProfile(profileName);

            // If we can't find the profile, respond with [404 Not Found]...

            if (execProfile == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] execution profile [{profileName}] not found.");
            }

            // If we found the profile, respond with [200 OK] + the profile definition...

            return Ok(ToApiModelContainer(execProfile, extensionId, exVersionId));
        }

        /// <summary>
        /// Creates a new extension service
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <param name="serviceApiModel">The extension service definition</param>
        /// <returns></returns>
        /// <response code="201">Extension service created.</response>
        /// <response code="400">Details included in response.</response>
        /// <response code="404">Extension or extension version not found.</response>
        /// <response code="409">Extension service already exists.</response>
        [HttpPost("{extensionId}/versions/{exVersionId}/services")]
        [ProducesResponseType(typeof(ApiModelContainer<ExtensionServiceApiModel>), 201)]
        public async Task<IActionResult> CreateExtensionServiceAsync([Required] string extensionId, [Required] string exVersionId, 
                                                                     [Required, FromBody] ExtensionServiceApiModel serviceApiModel)
        {
            // Validate the extension service API model...

            var errors = Validate(serviceApiModel);

            // If there were any validation errors, respond with [400 Bad Request]...

            if (errors.Any())
            {
                return BadRequest(string.Join(' ', errors));
            }

            // Try to find the specified extension...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we can't find the extension, respond with [404 Not Found]...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            // Try to find the specified extension version...

            var exVersion = extension.GetExtensionVersion(exVersionId);

            // If we can't find the extension version, respond with [404 Not Found]...

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            // Check to make sure that the specified service definition doesn't already exist...

            var serviceName = serviceApiModel.Name.ToLower();

            if (exVersion.SupportedServices.ContainsKey(serviceName))
            {
                // If the service definition already exists, respond with [409 Conflict]...

                return new ConflictObjectResult($"Extension [{extensionId}] version [{exVersionId}] service [{serviceApiModel.Name}] " +
                                                $"already exists at [{GetGetServiceUrl(extensionId, exVersionId, serviceName)}].");
            }

            // Add the service definition to the extension version and save the extension version...

            exVersion.SupportedServices[serviceName] = JObject.FromObject(serviceApiModel.ServiceConfiguration);

            await extensionRepository.SaveExtensionVersionAsync(exVersion);

            // Let the client know that the service definition has been created...

            return new CreatedResult(
                GetGetServiceUrl(extensionId, exVersionId, serviceName),
                ToApiModelContainer(extensionId, exVersionId, serviceName, exVersion.SupportedServices[serviceName]));
        }

        /// <summary>
        /// Gets all services associated with the specified extension version
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <returns></returns>
        /// <response code="200">Extension service(s) returned.</response>
        /// <response code="404">Extension or extension version not found.</response>
        [HttpGet("{extensionId}/versions/{exVersionId}/services")]
        [ProducesResponseType(typeof(IEnumerable<ApiModelContainer<ExtensionServiceApiModel>>), 200)]
        public async Task<IActionResult> GetAllServicesAsync([Required] string extensionId, [Required] string exVersionId)
        {
            // Try to find the specified extension...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we can't find the extension, respond with [404 Not Found]...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            // Try to find the specified extension version...

            var exVersion = extension.GetExtensionVersion(exVersionId);

            // If we can't find the extension version, respond with [404 Not Found]...

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            // If we do find the extension version, respond with [200 OK] + a list of all associated service definitions...

            return Ok(exVersion.SupportedServices.Select(s => ToApiModelContainer(extensionId, exVersionId, s.Key, s.Value)));
        }

        /// <summary>
        /// Gets the specified extension service
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <param name="serviceName">The extension service name</param>
        /// <returns></returns>
        /// <response code="200">Extension service returned.</response>
        /// <response code="404">Extension, extension version, or extension service not found.</response>
        [HttpGet("{extensionId}/versions/{exVersionId}/services/{serviceName}")]
        [ProducesResponseType(typeof(ApiModelContainer<ExtensionServiceApiModel>), 200)]
        public async Task<IActionResult> GetServiceAsync([Required] string extensionId, [Required] string exVersionId, [Required] string serviceName)
        {
            serviceName = WebUtility.UrlDecode(serviceName.ToLower());

            // Try to find the specified extension...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            // If we can't find the extension, respond with [404 Not Found]...

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            // Try to find the specified extension version...

            var exVersion = extension.GetExtensionVersion(exVersionId);

            // If we can't find the extension version, respond with [404 Not Found]...

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            // If we can't find the specified service definition, respond with [404 Not Found]...

            if (exVersion.SupportedServices.ContainsKey(serviceName) == false)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] service [{serviceName}] not found.");
            }

            // If we do find the service definition, respond with [200 OK] + the service definition...

            return Ok(ToApiModelContainer(extensionId, exVersionId, serviceName, exVersion.SupportedServices[serviceName]));
        }

        /// <summary>
        /// Deletes the specified extension service
        /// </summary>
        /// <param name="extensionId">The extension ID</param>
        /// <param name="exVersionId">The extension version ID</param>
        /// <param name="serviceName">The extension service name</param>
        /// <returns></returns>
        /// <response code="200"></response>
        [HttpDelete("{extensionId}/versions/{exVersionId}/services/{serviceName})")]
        public async Task<IActionResult> DeleteServiceAsync([Required] string extensionId, [Required] string exVersionId, [Required] string serviceName)
        {
            serviceName = WebUtility.UrlDecode(serviceName.ToLower());

            // Try to find the specified extension and extension version...

            var extension = await extensionRepository.GetExtensionAsync(extensionId);
            var exVersion = extension?.GetExtensionVersion(exVersionId);

            // If we find the specified service definition, remove it from and save the extension version...

            if (exVersion?.SupportedServices.ContainsKey(serviceName) == true)
            {
                exVersion.SupportedServices.Remove(serviceName);

                await extensionRepository.SaveExtensionVersionAsync(exVersion);
            }

            // Regardless of whether or not we found it, respond with [200 OK]...

            return Ok();
        }

        private IEnumerable<string> Validate(ExtensionApiModel extensionApiModel)
        {
            if (string.IsNullOrEmpty(extensionApiModel.Name))
            {
                yield return "Extension [name] is required.";
            }

            if (string.IsNullOrEmpty(extensionApiModel.PublisherName))
            {
                yield return "Extension [publisherName] is required.";
            }
        }

        private IEnumerable<string> Validate(ExtensionServiceApiModel serviceApiModel)
        {
            if (string.IsNullOrEmpty(serviceApiModel.Name))
            {
                yield return "Extension service [name] is required.";
            }
        }

        private IEnumerable<string> Validate(ExtensionVersionApiModel exVersionApiModel)
        {
            // Make sure that the provided extension version is in the right format...

            if (!Version.TryParse(exVersionApiModel.Version, out _))
            {
                yield return $"Extension version [version] [{exVersionApiModel.Version}] is invalid. " +
                              "[version] must include major, minor, and, optionally, build and revision numbers (e.g., 1.0, 1.1.0, 1.1.1.0).";
            }

            // Make sure that the request type URL is either not provided or is a valid absolute URL...

            if (!string.IsNullOrEmpty(exVersionApiModel.RequestTypeUrl) &&
                !Uri.TryCreate(exVersionApiModel.RequestTypeUrl, UriKind.Absolute, out _))
            {
                yield return $"Extension version [requestFormatUrl] [{exVersionApiModel.RequestTypeUrl}] is invalid. " +
                              "[requestFormatUrl] must be an absolute URL (e.g., http://foo.com/request/v1).";
            }

            // Make sure that the response type URL is either not provided or is a valid absolute URL...

            if (!string.IsNullOrEmpty(exVersionApiModel.ResponseTypeUrl) &&
                !Uri.TryCreate(exVersionApiModel.ResponseTypeUrl, UriKind.Absolute, out _))
            {
                yield return $"Extension version [responseFormatUrl] [{exVersionApiModel.ResponseTypeUrl}] is invalid. " +
                              "[responseFormatUrl] must be an absolute URL (e.g., http://foo.com/response/v1).";
            }
        }

        private IEnumerable<string> Validate(InputObjectApiModel objectApiModel)
        {
            if (string.IsNullOrEmpty(objectApiModel.Name))
            {
                yield return "Input object [name] is required.";
            }

            // Make sure that the object type URL is either not provided or is a valid absolute URL...

            if (!string.IsNullOrEmpty(objectApiModel.ObjectTypeUrl) &&
                !Uri.TryCreate(objectApiModel.ObjectTypeUrl, UriKind.Absolute, out _))
            {
                yield return $"Input object [objectTypeUrl] [{objectApiModel.ObjectTypeUrl}] is invalid. " +
                              "[objectTypeUrl] must be an absolute URL (e.g., http://foo.com/object/v1).";
            }
        }

        private IEnumerable<string> Validate(OutputObjectApiModel objectApiModel)
        {
            if (string.IsNullOrEmpty(objectApiModel.Name))
            {
                yield return "Output object [name] is required.";
            }

            // Make sure that the object type URL is either not provided or is a valid absolute URL...

            if (!string.IsNullOrEmpty(objectApiModel.ObjectTypeUrl) &&
                !Uri.TryCreate(objectApiModel.ObjectTypeUrl, UriKind.Absolute, out _))
            {
                yield return $"Output object [objectTypeUrl] [{objectApiModel.ObjectTypeUrl}] is invalid. " +
                              "[objectTypeUrl] must be an absolute URL (e.g., http://foo.com/object/v1).";
            }
        }

        private IEnumerable<string> Validate(ExecutionProfileApiModel profileApiModel)
        {
            if (string.IsNullOrEmpty(profileApiModel.Name))
            {
                yield return "Execution profile [name] is required.";
            }

            if (string.IsNullOrEmpty(profileApiModel.ExecutionModelName))
            {
                yield return "Execution profile [executionModel] is required.";
            }

            if (string.IsNullOrEmpty(profileApiModel.ObjectProviderName))
            {
                yield return "Execution profile [objectProvider] is required.";
            }

            if (string.IsNullOrEmpty(profileApiModel.ExecutionMode))
            {
                yield return "Execution profile [executionMode] is required; possible options are [direct] or [gateway].";
            }
            else
            {
                // Check to make sure that the execution model (either direct or gateway) that the client provided is valid.

                if (Enum.TryParse<ExecutionMode>(profileApiModel.ExecutionMode, true, out var executionMode))
                {
                    if (executionMode == ExecutionMode.Direct)
                    {
                        // If the direct execution model is specified, make sure that the client has provided an execution token duration...

                        if (string.IsNullOrEmpty(profileApiModel.DirectExecutionTokenDuration))
                        {
                            yield return "When [direct] [executionMode] is specified, execution profile [directExecutionTokenDuration] is required; " +
                                         "Expected [directExecutionTokenDuration] format is [hh:mm:ss] (e.g., [01:00:00]).";
                        }

                        // If the direct execution model is specified and an execution token duration is provided, make sure it's in the right format...

                        else if (TimeSpan.TryParse(profileApiModel.DirectExecutionTokenDuration, out _) == false)
                        {
                            yield return $"Execution profile [directExecutionTokenDuration] [{profileApiModel.DirectExecutionTokenDuration}] is invalid; " +
                                         "Expected [directExecutionTokenDuration] format is [hh:mm:ss] (e.g., [01:00:00]).";
                        }
                    }
                }
                else
                {
                    yield return $"Execution profile [executionMode] [{profileApiModel.ExecutionMode}] is invalid; possible options are [direct] or [gateway].";
                }
            }

            // Make sure that the client has provided at least one supported priority...

            if (profileApiModel.SupportedPriorities.None())
            {
                yield return "Execution profile must define at least one [supportedPriorities]; " +
                             "possible options are [low], [normal], and [high].";
            }

            // Otherwise, make sure that the provided priorities are valid...

            else
            {
                foreach (var priority in profileApiModel.SupportedPriorities)
                {
                    if (Enum.TryParse<ExecutionPriority>(priority, true, out _) == false)
                    {
                        yield return $"Execution profile [supportedPriorities] [{priority}] is invalid; " +
                                     "possible options are [low], [normal], and [high].";
                    }
                }
            }
        }

        private string GetPostExtensionUrl() =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions";

        private string GetDeleteExtensionUrl(string extensionId) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}";

        private string GetGetExtensionUrl(string extensionId) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}";

        private string GetPostExtensionVersionUrl(string extensionId) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions";

        private string GetGetExtensionVersionUrl(string extensionId, string exVersionId) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}";

        private string GetDeleteExtensionVersionUrl(string extensionId, string exVersionId) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}";

        private string GetGetExtensionVersionsUrl(string extensionId) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions";

        private string GetPostInputObjectUrl(string extensionId, string exVersionId) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}/objects/input";

        private string GetDeleteInputObjectUrl(string extensionId, string exVersionId, string objectName) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}/objects/input/{WebUtility.UrlEncode(objectName)}";

        private string GetGetInputObjectsUrl(string extensionId, string exVersionId) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}/objects/input";

        private string GetGetInputObjectUrl(string extensionId, string exVersionId, string objectName) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}/objects/input/{WebUtility.UrlEncode(objectName)}";

        private string GetPostOutputObjectUrl(string extensionId, string exVersionId) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}/objects/output";

        private string GetGetOutputObjectsUrl(string extensionId, string exVersionId) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}/objects/output";

        private string GetGetOutputObjectUrl(string extensionId, string exVersionId, string objectName) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}/objects/output/{WebUtility.UrlEncode(objectName)}";

        private string GetDeleteOutputObjectUrl(string extensionId, string exVersionId, string objectName) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}/objects/output/{WebUtility.UrlEncode(objectName)}";

        private string GetPostExecutionProfileUrl(string extensionId, string exVersionId) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}/profiles";

        private string GetGetExecutionProfilesUrl(string extensionId, string exVersionId) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}/profiles";

        private string GetGetExecutionProfileUrl(string extensionId, string exVersionId, string profileName) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}/profiles/{WebUtility.UrlEncode(profileName)}";

        private string GetDeleteExecutionProfileUrl(string extensionId, string exVersionId, string profileName) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}/profiles/{WebUtility.UrlEncode(profileName)}";

        private string GetPostServiceUrl(string extensionId, string exVersionId) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}/services";

        private string GetGetServicesUrl(string extensionId, string exVersionId) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}/services";

        private string GetGetServiceUrl(string extensionId, string exVersionId, string serviceName) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}/services/{WebUtility.UrlEncode(serviceName)}";

        private string GetDeleteServiceUrl(string extensionId, string exVersionId, string serviceName) =>
            (Request == null) ? (default) : $"{Request.Scheme}://{Request.Host}/extensions/{WebUtility.UrlEncode(extensionId)}/versions/{WebUtility.UrlEncode(exVersionId)}/services/{WebUtility.UrlEncode(serviceName)}";

        private ApiModelContainer<ExtensionApiModel> ToApiModelContainer(Extension extension) =>
            new ApiModelContainer<ExtensionApiModel>(extension.ToApiModel())
            {
                Links = new Dictionary<string, string>
                {
                    ["getExtension"] = GetGetExtensionUrl(extension.ExtensionId),
                    ["getExtensionVersions"] = GetGetExtensionVersionsUrl(extension.ExtensionId),
                    ["deleteExtension"] = GetDeleteExtensionUrl(extension.ExtensionId),
                    ["postNewExtension"] = GetPostExtensionUrl(),
                    ["postNewExtensionVersion"] = GetPostExtensionVersionUrl(extension.ExtensionId)
                }
            };

        private ApiModelContainer<ExtensionServiceApiModel> ToApiModelContainer(string extensionId, string exVersionId, string serviceName, JObject serviceConfig) =>
            new ApiModelContainer<ExtensionServiceApiModel>()
            {
                Model = new ExtensionServiceApiModel
                {
                    ExtensionId = extensionId,
                    ExtensionVersionId = exVersionId,
                    Name = serviceName,
                    ServiceConfiguration = serviceConfig?.ToObject<Dictionary<string, string>>()
                },
                Links = new Dictionary<string, string>
                {
                    ["getExtension"] = GetGetExtensionUrl(extensionId),
                    ["getExtensionVersion"] = GetGetExtensionVersionUrl(extensionId, exVersionId),
                    ["getAllExtensionVersions"] = GetGetExtensionVersionsUrl(extensionId),
                    ["getService"] = GetGetServiceUrl(extensionId, exVersionId, serviceName),
                    ["getAllServices"] = GetGetServicesUrl(extensionId, exVersionId),
                    ["deleteExtension"] = GetDeleteExtensionUrl(extensionId),
                    ["deleteExtensionVersion"] = GetDeleteExtensionVersionUrl(extensionId, exVersionId),
                    ["deleteService"] = GetDeleteServiceUrl(extensionId, exVersionId, serviceName),
                    ["postNewExtension"] = GetPostExtensionUrl(),
                    ["postNewExtensionVersion"] = GetPostExtensionVersionUrl(extensionId),
                    ["postNewService"] = GetPostServiceUrl(extensionId, exVersionId)
                }
            };

        private ApiModelContainer<ExtensionVersionApiModel> ToApiModelContainer(ExtensionVersion exVersion, string extensionId) =>
           new ApiModelContainer<ExtensionVersionApiModel>(exVersion.ToApiModel(extensionId))
           {
               Links = new Dictionary<string, string>
               {
                   ["getExtension"] = GetGetExtensionUrl(extensionId),
                   ["getExtensionVersion"] = GetGetExtensionVersionUrl(extensionId, exVersion.ExtensionVersionId),
                   ["getAllExtensionVersions"] = GetGetExtensionVersionsUrl(extensionId),
                   ["getAllExecutionProfiles"] = GetGetExecutionProfilesUrl(extensionId, exVersion.ExtensionVersionId),
                   ["getAllInputObjects"] = GetGetInputObjectsUrl(extensionId, exVersion.ExtensionVersionId),
                   ["getAllOutputObjects"] = GetGetOutputObjectsUrl(extensionId, exVersion.ExtensionVersionId),
                   ["getAllServices"] = GetGetServicesUrl(extensionId, exVersion.ExtensionVersionId),
                   ["deleteExtension"] = GetDeleteExtensionUrl(extensionId),
                   ["deleteExtensionVersion"] = GetDeleteExtensionVersionUrl(extensionId, exVersion.ExtensionVersionId),
                   ["postNewExtension"] = GetPostExtensionUrl(),
                   ["postNewExtensionVersion"] = GetPostExtensionVersionUrl(extensionId),
                   ["postNewExecutionProfile"] = GetPostExecutionProfileUrl(extensionId, exVersion.ExtensionVersionId),
                   ["postNewInputObject"] = GetPostInputObjectUrl(extensionId, exVersion.ExtensionVersionId),
                   ["postNewOutputObject"] = GetPostOutputObjectUrl(extensionId, exVersion.ExtensionVersionId),
                   ["postNewService"] = GetPostServiceUrl(extensionId, exVersion.ExtensionVersionId)
               }
           };

        private ApiModelContainer<InputObjectApiModel> ToApiModelContainer(ExtensionInputObject inputObject, string extensionId, string exVersionId) =>
            new ApiModelContainer<InputObjectApiModel>(inputObject.ToApiModel(extensionId, exVersionId))
            {
                Links = new Dictionary<string, string>
                {
                    ["getExtension"] = GetGetExtensionUrl(extensionId),
                    ["getExtensionVersion"] = GetGetExtensionVersionUrl(extensionId, exVersionId),
                    ["getAllExtensionVersions"] = GetGetExtensionVersionsUrl(extensionId),
                    ["getInputObject"] = GetGetInputObjectUrl(extensionId, exVersionId, inputObject.Name),
                    ["getAllInputObjects"] = GetGetInputObjectsUrl(extensionId, exVersionId),
                    ["deleteExtension"] = GetDeleteExtensionUrl(extensionId),
                    ["deleteExtensionVersion"] = GetDeleteExtensionVersionUrl(extensionId, exVersionId),
                    ["deleteInputObject"] = GetDeleteInputObjectUrl(extensionId, exVersionId, inputObject.Name),
                    ["postNewExtension"] = GetPostExtensionUrl(),
                    ["postNewExtensionVersion"] = GetPostExtensionVersionUrl(extensionId),
                    ["postNewInputObject"] = GetPostInputObjectUrl(extensionId, exVersionId)
                }
            };

        private ApiModelContainer<OutputObjectApiModel> ToApiModelContainer(ExtensionOutputObject outputObject, string extensionId, string exVersionId) =>
            new ApiModelContainer<OutputObjectApiModel>(outputObject.ToApiModel(extensionId, exVersionId))
            {
                Links = new Dictionary<string, string>
                {
                    ["getExtension"] = GetGetExtensionUrl(extensionId),
                    ["getExtensionVersion"] = GetGetExtensionVersionUrl(extensionId, exVersionId),
                    ["getAllExtensionVersions"] = GetGetExtensionVersionsUrl(extensionId),
                    ["getOutputObject"] = GetGetOutputObjectUrl(extensionId, exVersionId, outputObject.Name),
                    ["getAllOutputObjects"] = GetGetOutputObjectsUrl(extensionId, exVersionId),
                    ["deleteExtension"] = GetDeleteExtensionUrl(extensionId),
                    ["deleteExtensionVersion"] = GetDeleteExtensionVersionUrl(extensionId, exVersionId),
                    ["deleteOutputObject"] = GetDeleteOutputObjectUrl(extensionId, exVersionId, outputObject.Name),
                    ["postNewExtension"] = GetPostExtensionUrl(),
                    ["postNewExtensionVersion"] = GetPostExtensionVersionUrl(extensionId),
                    ["postNewOutputObject"] = GetPostOutputObjectUrl(extensionId, exVersionId)
                }
            };

        private ApiModelContainer<ExecutionProfileApiModel> ToApiModelContainer(ExecutionProfile execProfile, string extensionId, string exVersionId) =>
           new ApiModelContainer<ExecutionProfileApiModel>(execProfile.ToApiModel(extensionId, exVersionId))
           {
               Links = new Dictionary<string, string>
               {
                   ["getExtension"] = GetGetExtensionUrl(extensionId),
                   ["getExtensionVersion"] = GetGetExtensionVersionUrl(extensionId, exVersionId),
                   ["getAllExtensionVersions"] = GetGetExtensionVersionsUrl(extensionId),
                   ["getExecutionProfile"] = GetGetExecutionProfileUrl(extensionId, exVersionId, execProfile.ProfileName),
                   ["getAllExecutionProfiles"] = GetGetExecutionProfilesUrl(extensionId, exVersionId),
                   ["deleteExtension"] = GetDeleteExtensionUrl(extensionId),
                   ["deleteExtensionVersion"] = GetDeleteExtensionVersionUrl(extensionId, exVersionId),
                   ["deleteExecutionProfile"] = GetDeleteExecutionProfileUrl(extensionId, exVersionId, execProfile.ProfileName),
                   ["postNewExtension"] = GetPostExtensionUrl(),
                   ["postNewExtensionVersion"] = GetPostExtensionVersionUrl(extensionId),
                   ["postNewExecutionProfile"] = GetPostExecutionProfileUrl(extensionId, exVersionId)
               }
           };
    }
}