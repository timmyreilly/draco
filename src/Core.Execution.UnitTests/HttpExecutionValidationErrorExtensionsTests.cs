using Draco.Core.Execution.Extensions;
using Draco.Core.Execution.Models;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using System;
using Xunit;

namespace Draco.Core.Execution.UnitTests
{
    public class HttpExecutionValidationErrorExtensionsTests
    {
        [Fact]
        public void ToCoreModel_GivenValidHttpValidationError_FieldsShouldMatch()
        {
            var httpValidationError = new HttpExecutionValidationError
            {
                ErrorCode = "Wrong",
                ErrorId = Guid.NewGuid().ToString(),
                ErrorMessage = "You did it wrong"
            };

            httpValidationError.ErrorData = JObject.FromObject(httpValidationError);

            var coreModel = httpValidationError.ToCoreModel();

            coreModel.ErrorCode.Should().Be(httpValidationError.ErrorCode);
            coreModel.ErrorId.Should().Be(httpValidationError.ErrorId);
            coreModel.ErrorMessage.Should().Be(httpValidationError.ErrorMessage);
            coreModel.ErrorData.Should().BeEquivalentTo(httpValidationError.ErrorData);
        }
    }
}
