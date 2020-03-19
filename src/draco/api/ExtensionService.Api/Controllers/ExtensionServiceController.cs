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
            var errors = execRequestApiModel.ValidateApiModel().ToList();

            if (errors.Any())
            {
                return BadRequest($"[{errors.Count}] errors occurred while attempting to process your request: {string.Join(' ', errors)}");
            }

            var execRequest = execRequestApiModel.ToCoreModel();
            var execServiceConfig = await execServiceProvider.GetServiceConfigurationAsync(execRequest);

            if (execServiceConfig == null)
            {
                return NoContent();
            }
            else
            {
                return Ok(execServiceConfig);
            }
        }
    }
}
