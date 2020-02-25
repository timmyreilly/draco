// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Api.InternalModels;
using Draco.Api.InternalModels.Extensions;
using Draco.Core.Execution.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Draco.ExecutionAdapter.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("execution-adapter")]
    public class ExecutionAdapterController : ControllerBase
    {
        private readonly IExecutionRequestRouter execRequestRouter;

        public ExecutionAdapterController(IExecutionRequestRouter execRequestRouter)
        {
            this.execRequestRouter = execRequestRouter;
        }

        /// <summary>
        /// Posts an internal execution request to a back-end execution adapter.
        /// </summary>
        /// <param name="execRequestApiModel">The internal execution request</param>
        /// <returns></returns>
        /// <response code="200">Internal execution request has been successfully posted to a back-end execution adapter.</response>
        /// <response code="400">See response contents for additional information.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ExecutionResponseApiModel), 200)]
        public async Task<IActionResult> ExecuteAsync([FromBody] ExecutionRequestApiModel execRequestApiModel)
        {
            var errors = execRequestApiModel.ValidateApiModel().ToList();

            if (errors.Any())
            {
                return BadRequest($"[{errors.Count}] errors occurred while attempting to process your request: {string.Join(' ', errors)}");
            }

            var execRequest = execRequestApiModel.ToCoreModel();
            var execContext = await execRequestRouter.RouteRequestAsync(execRequest, CancellationToken.None);

            return Ok(execContext.ToApiModel());
        }
    }
}
