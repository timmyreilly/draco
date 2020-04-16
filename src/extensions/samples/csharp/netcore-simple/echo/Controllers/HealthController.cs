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
    [Route("Health")]
    [ProducesResponseType(typeof(HttpExecutionResponse), 200)]
    public class HealthController : ControllerBase
    {
        private readonly ILogger _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            using(_logger.BeginScope("Health probe get"))
            {
                return Ok(DateTime.UtcNow.ToString());
            }
        }

    }

}