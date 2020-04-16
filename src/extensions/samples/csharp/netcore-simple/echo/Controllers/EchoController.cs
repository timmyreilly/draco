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
        private readonly ILogger _logger;

        public EchoController(ILogger<EchoController> logger)
        {
            _logger = logger;
        }

        // POST echo
        [HttpPost]
        [ProducesResponseType(typeof(HttpExecutionResponse), 200)]
        public IActionResult PostEcho(HttpExecutionRequest execRequest)
        {
            using( _logger.BeginScope("Echo Post") )
            {
                HttpExecutionResponse httpExecResponse = new HttpExecutionResponse();
                httpExecResponse.ExecutionId = execRequest.ExecutionId;
                httpExecResponse.ResponseData = JObject.FromObject(execRequest);
                return Ok(httpExecResponse);
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            using(_logger.BeginScope("Echo probe get"))
            {
                return Ok("Please POST the following snippet: \r\n{ \"requestData\": { \"request\": \"some request data\" }, \"properties\": { \"message\": \"hello, world!\" } }");
            }
        }

    }
}
