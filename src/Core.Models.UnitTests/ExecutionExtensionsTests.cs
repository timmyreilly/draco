using Draco.Core.Models.Enumerations;
using Draco.Core.Models.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Draco.Core.Models.UnitTests
{
    public class ExecutionExtensionsTests
    {
        [Fact]
        public void ToEvent_GivenExecution_ShouldReturnEquivalentExecutionUpdateEvent()
        {
            var execution = new Execution
            {
                ExecutionId = Guid.NewGuid().ToString(),
                Executor = new ExecutorContext
                {
                    TenantId = Guid.NewGuid().ToString(),
                    UserId = Guid.NewGuid().ToString()
                },
                ExtensionId = Guid.NewGuid().ToString(),
                ExtensionVersionId = Guid.NewGuid().ToString(),
                PercentComplete = 1,
                Priority = ExecutionPriority.High,
                Status = ExecutionStatus.Succeeded,
                StatusMessage = "All done!",
                LastUpdatedDateTimeUtc = DateTime.UtcNow
            };

            var updateEvent = execution.ToEvent();

            updateEvent.Should().NotBeNull();
            updateEvent.ExecutionId.Should().Be(execution.ExecutionId);
            updateEvent.Executor.Should().Be(execution.Executor);
            updateEvent.ExtensionId.Should().Be(execution.ExtensionId);
            updateEvent.ExtensionVersionId.Should().Be(execution.ExtensionVersionId);
            updateEvent.PercentageComplete.Should().Be(execution.PercentComplete);
            updateEvent.Priority.Should().Be(execution.Priority);
            updateEvent.Status.Should().Be(execution.Status);
            updateEvent.StatusMessage.Should().Be(execution.StatusMessage);
            updateEvent.UpdateDateTimeUtc.Should().Be(execution.LastUpdatedDateTimeUtc);
        }
    }
}
