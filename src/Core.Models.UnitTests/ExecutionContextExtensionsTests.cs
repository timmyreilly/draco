// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models.Enumerations;
using Draco.Core.Models.Extensions;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Draco.Core.Models.UnitTests
{
    public class ExecutionContextExtensionsTests
    {
        [Fact]
        public void ToExecutionUpdate_GivenExecutionContext_ShouldReturnEquivalentExecutionUpdate()
        {
            var execContext = CreateTestExecutionContext();

            execContext.ResultData = JObject.FromObject(execContext);

            var execUpdate = execContext.ToExecutionUpdate();

            execUpdate.Should().NotBeNull();
            execUpdate.PercentComplete.Should().Be(execContext.PercentComplete);
            execUpdate.Status.Should().Be(execContext.Status.ToString());
            execUpdate.StatusMessage.Should().Be(execContext.StatusMessage);
            execUpdate.ResultData.Should().BeEquivalentTo(execContext.ResultData);
            execUpdate.ValidationErrors.Should().BeEquivalentTo(execContext.ValidationErrors);
            execUpdate.ProvidedOutputObjects.Should().BeEquivalentTo(execContext.ProvidedOutputObjects);
            execUpdate.StatusUpdateKey.Should().Be(execContext.StatusUpdateKey);
            execUpdate.LastUpdatedDateTimeUtc.Should().Be(execContext.LastUpdatedDateTimeUtc);
        }

        [Fact]
        public void UpdateStatus_GivenNullExecutionContext_ShouldThrowException()
        {
            Action act = () => (null as ExecutionContext).UpdateStatus(ExecutionStatus.Failed);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateStatus_GivenSameStatus_ShouldNotUpdateLastUpdatedDateTimeUtc()
        {
            var execContext = CreateTestExecutionContext();
            var originalLastUpdatedDt = execContext.LastUpdatedDateTimeUtc;

            execContext.Status = ExecutionStatus.Processing;

            var execUpdate = execContext.UpdateStatus(ExecutionStatus.Processing);

            execUpdate.LastUpdatedDateTimeUtc.Should().Be(originalLastUpdatedDt);
        }

        [Fact]
        public void UpdateStatus_GivenDifferentStatus_ShouldUpdateLastUpdatedDateTimeUtc()
        {
            var execContext = CreateTestExecutionContext();
            var originalLastUpdatedDt = execContext.LastUpdatedDateTimeUtc;

            execContext.Status = ExecutionStatus.Processing;

            var execUpdate = execContext.UpdateStatus(ExecutionStatus.Succeeded);

            execUpdate.LastUpdatedDateTimeUtc.Should().BeAfter(originalLastUpdatedDt);
        }

        private ExecutionContext CreateTestExecutionContext() =>
            new ExecutionContext
            {
                ExecutionId = Guid.NewGuid().ToString(),
                PercentComplete = 1,
                Status = ExecutionStatus.ValidationFailed,
                StatusMessage = "It failed!",
                ValidationErrors = new List<ExecutionValidationError>
                {
                    new ExecutionValidationError
                    {
                        ErrorCode = "Wrong",
                        ErrorId = Guid.NewGuid().ToString(),
                        ErrorMessage = "You did it wrong!"
                    }
                },
                ProvidedOutputObjects = new List<string> { "OutputObjectA" },
                StatusUpdateKey = Guid.NewGuid().ToString(),
                CreatedDateTimeUtc = DateTime.UtcNow.AddMinutes(-5),
                LastUpdatedDateTimeUtc = DateTime.UtcNow.AddMinutes(-5)
            };
    }
}
