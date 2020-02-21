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
            var errors = Validate(extensionApiModel);

            if (errors.Any())
            {
                return BadRequest(string.Join(' ', errors));
            }

            var extension = extensionApiModel.ToCoreModel();

            await extensionRepository.SaveExtensionAsync(extension);

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
            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

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
            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            if (extension?.IsActive == true)
            {
                extension.IsActive = false;

                await extensionRepository.SaveExtensionAsync(extension);
            }

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
            var errors = Validate(exVersionApiModel);

            if (errors.Any())
            {
                return BadRequest(string.Join(' ', errors));
            }

            var version = new Version(exVersionApiModel.Version);
            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            var exVersion = extension.ExtensionVersions.SingleOrDefault(ev => Version.Parse(ev.Version) == version);

            if (exVersion != null)
            {
                return new ConflictObjectResult($"Extension [{extensionId}] version [{version}] already exists at " +
                                                $"[{GetGetExtensionVersionUrl(extensionId, exVersion.ExtensionVersionId)}].");
            }

            exVersion = exVersionApiModel.ToCoreModel(extensionId);

            await extensionRepository.SaveExtensionVersionAsync(exVersion);

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
            var extension = await extensionRepository.GetExtensionAsync(extensionId);
            var exVersion = extension?.GetExtensionVersion(exVersionId);

            if (exVersion?.IsActive == true)
            {
                exVersion.IsActive = false;

                await extensionRepository.SaveExtensionVersionAsync(exVersion);
            }

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
            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

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
            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            var exVersion = extension.GetExtensionVersion(exVersionId);

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

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
            var errors = Validate(inputObjectApiModel);

            if (errors.Any())
            {
                return BadRequest(string.Join(' ', errors));
            }

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            var exVersion = extension.GetExtensionVersion(exVersionId);

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            var inputObjectName = inputObjectApiModel.Name.ToLower();
            var inputObject = exVersion.InputObjects.SingleOrDefault(io => (io.Name == inputObjectName));

            if (inputObject != null)
            {
                return new ConflictObjectResult($"Extension [{extensionId}] version [{exVersionId}] input object [{inputObjectApiModel.Name}] " +
                                                $"already exists at [{GetGetInputObjectUrl(extensionId, exVersionId, inputObjectName)}].");
            }

            inputObject = inputObjectApiModel.ToCoreModel();

            exVersion.InputObjects.Add(inputObject);

            await extensionRepository.SaveExtensionVersionAsync(exVersion);

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
            objectName = WebUtility.UrlDecode(objectName);

            var extension = await extensionRepository.GetExtensionAsync(extensionId);
            var exVersion = extension?.GetExtensionVersion(exVersionId);
            var inputObject = exVersion?.GetInputObject(objectName.ToLower());

            if (inputObject != null)
            {
                exVersion.InputObjects.Remove(inputObject);

                await extensionRepository.SaveExtensionVersionAsync(exVersion);
            }

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
            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            var exVersion = extension.GetExtensionVersion(exVersionId);

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

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

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            var exVersion = extension.GetExtensionVersion(exVersionId);

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            var inputObject = exVersion.InputObjects.SingleOrDefault(io => io.Name == objectName.ToLower());

            if (inputObject == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] input object [{objectName}] not found.");
            }

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
            var errors = Validate(outputObjectApiModel);

            if (errors.Any())
            {
                return BadRequest(string.Join(' ', errors));
            }

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            var exVersion = extension.GetExtensionVersion(exVersionId);

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            var outputObjectName = outputObjectApiModel.Name.ToLower();
            var outputObject = exVersion.OutputObjects.SingleOrDefault(oo => oo.Name == outputObjectName);

            if (outputObject != null)
            {
                return new ConflictObjectResult($"Extension [{extensionId}] version [{exVersionId}] output object [{outputObjectApiModel.Name}] " +
                                                $"already exists at [{GetGetOutputObjectUrl(extensionId, exVersionId, outputObjectName)}].");
            }

            outputObject = outputObjectApiModel.ToCoreModel();

            exVersion.OutputObjects.Add(outputObject);

            await extensionRepository.SaveExtensionVersionAsync(exVersion);

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

            var extension = await extensionRepository.GetExtensionAsync(extensionId);
            var exVersion = extension?.GetExtensionVersion(exVersionId);
            var outputObject = exVersion?.GetOutputObject(objectName.ToLower());

            if (outputObject != null)
            {
                exVersion.OutputObjects.Remove(outputObject);

                await extensionRepository.SaveExtensionVersionAsync(exVersion);
            }

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
            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            var exVersion = extension.GetExtensionVersion(exVersionId);

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

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

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            var exVersion = extension.GetExtensionVersion(exVersionId);

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            var outputObject = exVersion.OutputObjects.SingleOrDefault(oo => oo.Name == objectName.ToLower());

            if (outputObject == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] output object [{objectName}] not found.");
            }

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
            var errors = Validate(execProfileApiModel);

            if (errors.Any())
            {
                return BadRequest(string.Join(' ', errors));
            }

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            var exVersion = extension.GetExtensionVersion(exVersionId);

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            var execProfileName = execProfileApiModel.Name.ToLower();
            var execProfile = exVersion.ExecutionProfiles.SingleOrDefault(p => p.ProfileName == execProfileName);

            if (execProfile != null)
            {
                return new ConflictObjectResult($"Extension [{extensionId}] version [{exVersionId}] execution profile [{execProfileApiModel.Name}] " +
                                                $"already exists at [{GetGetExecutionProfileUrl(extensionId, exVersionId, execProfileName)}].");
            }

            execProfile = execProfileApiModel.ToCoreModel();

            exVersion.ExecutionProfiles.Add(execProfile);

            await extensionRepository.SaveExtensionVersionAsync(exVersion);

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

            var extension = await extensionRepository.GetExtensionAsync(extensionId);
            var exVersion = extension?.GetExtensionVersion(exVersionId);
            var exProfile = exVersion?.GetExecutionProfile(profileName);

            if (exProfile != null)
            {
                exVersion.ExecutionProfiles.Remove(exProfile);

                await extensionRepository.SaveExtensionVersionAsync(exVersion);
            }

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
            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            var exVersion = extension.GetExtensionVersion(exVersionId);

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

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

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            var exVersion = extension.GetExtensionVersion(exVersionId);

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            var execProfile = exVersion.GetExecutionProfile(profileName);

            if (execProfile == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] execution profile [{profileName}] not found.");
            }

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
            var errors = Validate(serviceApiModel);

            if (errors.Any())
            {
                return BadRequest(string.Join(' ', errors));
            }

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            var exVersion = extension.GetExtensionVersion(exVersionId);

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            var serviceName = serviceApiModel.Name.ToLower();

            if (exVersion.SupportedServices.ContainsKey(serviceName))
            {
                return new ConflictObjectResult($"Extension [{extensionId}] version [{exVersionId}] service [{serviceApiModel.Name}] " +
                                                $"already exists at [{GetGetServiceUrl(extensionId, exVersionId, serviceName)}].");
            }

            exVersion.SupportedServices[serviceName] = JObject.FromObject(serviceApiModel.ServiceConfiguration);

            await extensionRepository.SaveExtensionVersionAsync(exVersion);

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
            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            var exVersion = extension.GetExtensionVersion(exVersionId);

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

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

            var extension = await extensionRepository.GetExtensionAsync(extensionId);

            if (extension == null)
            {
                return NotFound($"Extension [{extensionId}] not found.");
            }

            var exVersion = extension.GetExtensionVersion(exVersionId);

            if (exVersion == null)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] not found.");
            }

            if (exVersion.SupportedServices.ContainsKey(serviceName) == false)
            {
                return NotFound($"Extension [{extensionId}] version [{exVersionId}] service [{serviceName}] not found.");
            }

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

            var extension = await extensionRepository.GetExtensionAsync(extensionId);
            var exVersion = extension?.GetExtensionVersion(exVersionId);

            if (exVersion?.SupportedServices.ContainsKey(serviceName) == true)
            {
                exVersion.SupportedServices.Remove(serviceName);

                await extensionRepository.SaveExtensionVersionAsync(exVersion);
            }

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
            if (!Version.TryParse(exVersionApiModel.Version, out _))
            {
                yield return $"Extension version [version] [{exVersionApiModel.Version}] is invalid. " +
                              "[version] must include major, minor, and, optionally, build and revision numbers (e.g., 1.0, 1.1.0, 1.1.1.0).";
            }

            if (!string.IsNullOrEmpty(exVersionApiModel.RequestTypeUrl) &&
                !Uri.TryCreate(exVersionApiModel.RequestTypeUrl, UriKind.Absolute, out _))
            {
                yield return $"Extension version [requestFormatUrl] [{exVersionApiModel.RequestTypeUrl}] is invalid. " +
                              "[requestFormatUrl] must be an absolute URL (e.g., http://foo.com/request/v1).";
            }

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
                if (Enum.TryParse<ExecutionMode>(profileApiModel.ExecutionMode, true, out var executionMode))
                {
                    if (executionMode == ExecutionMode.Direct)
                    {
                        if (string.IsNullOrEmpty(profileApiModel.DirectExecutionTokenDuration))
                        {
                            yield return "When [direct] [executionMode] is specified, execution profile [directExecutionTokenDuration] is required; " +
                                         "Expected [directExecutionTokenDuration] format is [hh:mm:ss] (e.g., [01:00:00]).";
                        }
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

            if (profileApiModel.SupportedPriorities.None())
            {
                yield return "Execution profile must define at least one [supportedPriorities]; " +
                             "possible options are [low], [normal], and [high].";
            }
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