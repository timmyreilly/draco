// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Draco.Core.Execution.Models;

namespace echo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EchoController : ControllerBase
    {

        public EchoController(ILogger<EchoController> logger)
        {
        }

        // POST echo
        [HttpPost]
        [ProducesResponseType(typeof(HttpExecutionResponse), 200)]
        public async Task<IActionResult> PostEcho(HttpExecutionRequest execRequest)
        {
            HttpExecutionResponse httpExecResponse = new HttpExecutionResponse();
            httpExecResponse.ExecutionId = execRequest.ExecutionId;
            httpExecResponse.ResponseData = JObject.FromObject(execRequest);
            return Ok(httpExecResponse);
        }

    }
}
