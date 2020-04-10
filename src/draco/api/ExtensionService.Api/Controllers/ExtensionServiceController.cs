// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Api.InternalModels;
using Draco.Api.InternalModels.Extensions;
using Draco.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ExtensionService.Api.Controllers
{
    
    /// <summary>
    /// This controller is essentially an API wrapper around /src/core/Services/Interfaces/IExecutionServiceProvider.cs.
    /// The execution API interacts with it via /src/api/Api.Proxies/ProxyExecutionServiceProvider.cs.
    /// For more information on execution services, see /doc/architecture/execution-services.md.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("extension-service")]
    public class ExtensionServiceController : ControllerBase
    {
        private readonly IExecutionServiceProvider execServiceProvider;

        public ExtensionServiceController(IExecutionServiceProvider execServiceProvider)
        {
            this.execServiceProvider = execServiceProvider;
        }

        [HttpPost("config-request")]
        public async Task<IActionResult> GetServiceConfigurationAsync([FromBody] ExecutionRequestApiModel execRequestApiModel)
        {
            // Validate the execution request API model...

            var errors = execRequestApiModel.ValidateApiModel().ToList();

            // If we ran into any validation errors, respond with [400 Bad Request] + detailed error description...

            if (errors.Any())
            {
                return BadRequest($"[{errors.Count}] errors occurred while attempting to process your request: {string.Join(' ', errors)}");
            }

            // Get any service context(s) from the execution service provider...

            var execRequest = execRequestApiModel.ToCoreModel();
            var execServiceConfig = await execServiceProvider.GetServiceConfigurationAsync(execRequest);

            if (execServiceConfig == null)
            {
                // If there are no available services, respond with [204 No Content]...

                return NoContent();
            }
            else
            {
                // If there are available services, respond with [200 OK] + service context(s) for execution...

                return Ok(execServiceConfig);
            }
        }
    }
}
