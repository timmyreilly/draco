// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Api.InternalModels;
using Draco.Api.InternalModels.Extensions;
using Draco.Core.ObjectStorage.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Draco.ObjectStorageProvider.Api.Controllers
{
    /// <summary>
    /// This controller is essentially an API wrapper around /src/core/ObjectStorage/Interfaces/IInputObjectAccessorProvider.cs 
    /// and /src/core/ObjectStorage/Interfaces/IOutputObjectAccessorProvider.cs.
    /// 
    /// Other processes (execution API, execution adapter API, and remote execution agent) interact with it via 
    /// /src/api/Api.Proxies/ProxyInputObjectAccessorProvider.cs and /src/api/Api.Proxies/ProxyOutputObjectAccessorProvider.cs respectively.
    /// 
    /// For more information on execution objects, see /doc/architecture/execution-objects.md.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("object-storage")]
    public class ObjectStorageController : ControllerBase
    {
        private readonly IInputObjectAccessorProvider ioAccessorProvider;
        private readonly IOutputObjectAccessorProvider ooAccessorProvider;

        public ObjectStorageController(IInputObjectAccessorProvider ioAccessorProvider,
                                       IOutputObjectAccessorProvider ooAccessorProvider)
        {
            this.ioAccessorProvider = ioAccessorProvider;
            this.ooAccessorProvider = ooAccessorProvider;
        }

        /// <summary>
        /// Calculates a readable input object accessor
        /// </summary>
        /// <param name="requestApiModel">The input object accessor request</param>
        /// <returns></returns>
        /// <response code="200">Readable input object acccessor returned.</response>
        /// <response code="400">See response content for further details.</response>
        [HttpPost("input/readable")]
        public async Task<IActionResult> GetReadableInputObjectAccessorAsync([FromBody] InputObjectAccessorRequestApiModel requestApiModel)
        {
            // Validate the input object request API model...

            var errors = requestApiModel.ValidateApiModel().ToList();

            // If we encounter any validation errors, respond with [400 Bad Request] + detailed error description...

            if (errors.Any())
            {
                return BadRequest($"[{errors.Count}] error(s) occurred while attempting to process your request: {string.Join(' ', errors)}");
            }

            // Conver the accessor request API model to its core model counterpart and request the input object accessor...

            var ioAccessorRequest = requestApiModel.ToCoreModel(requestApiModel.ObjectName);
            var ioAccessor = await ioAccessorProvider.GetReadableAccessorAsync(ioAccessorRequest);

            // Respond with [200 OK] + the input object accessor...

            return Ok(ioAccessor);
        }

        /// <summary>
        /// Calculates a writable input object accessor
        /// </summary>
        /// <param name="requestApiModel">The input object accessor request</param>
        /// <returns></returns>
        /// <response code="200">Writable input object acccessor returned.</response>
        /// <response code="400">See response content for further details.</response>
        [HttpPost("input/writable")]
        public async Task<IActionResult> CalculateWriteableInputObjectAccessorAsync ([FromBody] InputObjectAccessorRequestApiModel requestApiModel)
        {
            // Validate the input object request API model...

            var errors = requestApiModel.ValidateApiModel().ToList();

            // If we encounter any validation errors, respond with [400 Bad Request] + detailed error description...

            if (errors.Any())
            {
                return BadRequest($"[{errors.Count}] error(s) occurred while attempting to process your request: {string.Join(' ', errors)}");
            }

            // Convert the input object accessor request to its core model counterpart and attempt to obtain an object accessor...

            var ioAccessorRequest = requestApiModel.ToCoreModel(requestApiModel.ObjectName);
            var ioAccessor = await ioAccessorProvider.GetWritableAccessorAsync(ioAccessorRequest);

            // Respond with [200 OK] + the accessor...

            return Ok(ioAccessor);
        }

        /// <summary>
        /// Calculates a readable output object accessor
        /// </summary>
        /// <param name="requestApiModel">The output object accessor request</param>
        /// <returns></returns>
        /// <response code="200">Readable output object acccessor returned.</response>
        /// <response code="400">See response content for further details.</response>
        [HttpPost("output/readable")]
        public async Task<IActionResult> CalculateReadableOutputObjectAccessorAsync([FromBody] OutputObjectAccessorRequestApiModel requestApiModel)
        {
            // Validate the output object request API model...

            var errors = requestApiModel.ValidateApiModel().ToList();

            // If we encounter any validation errors, respond with [400 Bad Request] + detailed error description...

            if (errors.Any())
            {
                return BadRequest($"[{errors.Count}] error(s) occurred while attempting to process your request: {string.Join(' ', errors)}");
            }

            // Convert the output object accessor request to its core model counterpart and attempt to obtain an object accessor...

            var ooAccessorRequest = requestApiModel.ToCoreModel(requestApiModel.ObjectName);
            var ooAccessor = await ooAccessorProvider.GetReadableAccessorAsync(ooAccessorRequest);

            // Repond with [200 OK] + the accessor...

            return Ok(ooAccessor);
        }

        /// <summary>
        /// Calculates a writable output object accessor
        /// </summary>
        /// <param name="requestApiModel">The output object accessor request</param>
        /// <returns></returns>
        /// <response code="200">Writable output object acccessor returned.</response>
        /// <response code="400">See response content for further details.</response>
        [HttpPost("output/writable")]
        public async Task<IActionResult> CalculateWritableOutputObjectAccessorAsync([FromBody] OutputObjectAccessorRequestApiModel requestApiModel)
        {
            // Validate the output object accessor request API model...

            var errors = requestApiModel.ValidateApiModel().ToList();

            // If we encounter any validation errors, respond with [400 Bad Request] + detailed error description...

            if (errors.Any())
            {
                return BadRequest($"[{errors.Count}] error(s) occurred while attempting to process your request: {string.Join(' ', errors)}");
            }

            // Convert the output object accessor request to its core model counterpart and attempt to obtain an object accessor...

            var ooAccessorRequest = requestApiModel.ToCoreModel(requestApiModel.ObjectName);
            var ooAccessor = await ooAccessorProvider.GetWritableAccessorAsync(ooAccessorRequest);

            // Repond with [200 OK] + the accessor...

            return Ok(ooAccessor);
        }
    }
}
