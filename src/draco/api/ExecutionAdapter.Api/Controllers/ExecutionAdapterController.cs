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
    /// <summary>
    /// This controller is essentially an API wrapper around /src/core/Execution/Interfaces/IExecutionAdapter.cs.
    /// The execution API interacts with it via /src/api/Api.Proxies/ProxyExecutionServiceProvider.cs.
    /// For more information on execution models, see /doc/architecture/execution-models.md.
    /// </summary>
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
            // Validate the provided execution request...

            var errors = execRequestApiModel.ValidateApiModel().ToList();

            // If the request is invalid, respond with [400 Bad Request] + detailed error description...

            if (errors.Any())
            {
                return BadRequest($"[{errors.Count}] errors occurred while attempting to process your request: {string.Join(' ', errors)}");
            }

            // Convert the execution request API model to a core model and hand it off to the execution pipeline.
            // For more information on the execution pipeline, see /doc/architecture/execution-pipeline.md.

            var execRequest = execRequestApiModel.ToCoreModel();
            var execContext = await execRequestRouter.RouteRequestAsync(execRequest, CancellationToken.None);

            // At this point, even if the execution itself failed, we did our job.
            // Respond with [200 OK] + an execution update...

            return Ok(execContext.ToApiModel());
        }
    }
}
