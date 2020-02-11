using Draco.Api.InternalModels;
using Draco.Api.InternalModels.Extensions;
using Draco.Core.ObjectStorage.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Draco.ObjectStorageProvider.Api.Controllers
{
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
            var errors = requestApiModel.ValidateApiModel().ToList();

            if (errors.Any())
            {
                return BadRequest($"[{errors.Count}] error(s) occurred while attempting to process your request: {string.Join(' ', errors)}");
            }

            var ioAccessorRequest = requestApiModel.ToCoreModel(requestApiModel.ObjectName);
            var ioAccessor = await ioAccessorProvider.GetReadableAccessorAsync(ioAccessorRequest);

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
            var errors = requestApiModel.ValidateApiModel().ToList();

            if (errors.Any())
            {
                return BadRequest($"[{errors.Count}] error(s) occurred while attempting to process your request: {string.Join(' ', errors)}");
            }

            var ioAccessorRequest = requestApiModel.ToCoreModel(requestApiModel.ObjectName);
            var ioAccessor = await ioAccessorProvider.GetWritableAccessorAsync(ioAccessorRequest);

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
            var errors = requestApiModel.ValidateApiModel().ToList();

            if (errors.Any())
            {
                return BadRequest($"[{errors.Count}] error(s) occurred while attempting to process your request: {string.Join(' ', errors)}");
            }

            var ooAccessorRequest = requestApiModel.ToCoreModel(requestApiModel.ObjectName);
            var ooAccessor = await ooAccessorProvider.GetReadableAccessorAsync(ooAccessorRequest);

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
            var errors = requestApiModel.ValidateApiModel().ToList();

            if (errors.Any())
            {
                return BadRequest($"[{errors.Count}] error(s) occurred while attempting to process your request: {string.Join(' ', errors)}");
            }

            var ooAccessorRequest = requestApiModel.ToCoreModel(requestApiModel.ObjectName);
            var ooAccessor = await ooAccessorProvider.GetWritableAccessorAsync(ooAccessorRequest);

            return Ok(ooAccessor);
        }
    }
}
