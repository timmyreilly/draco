// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Interfaces;
using Draco.Core.Execution.Options;
using Draco.Core.Execution.Processors;
using Draco.Core.Models;
using Draco.Core.Models.Enumerations;
using Draco.Core.Models.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Draco.Core.Execution.UnitTests
{
    public class ExecutionProcessorTests
    {
        readonly IExecutionProcessorOptions defaultProcessorOptions;

        public ExecutionProcessorTests()
        {
            defaultProcessorOptions = CreateDefaultProcessorOptions();
        }

        [Fact]
        public void ProcessRequestAsync_NullExecutionRequest_ShouldThrowException()
        {
            var mockExecAdapter = new Mock<IExecutionAdapter>();
            var mockLogger = new Mock<ILogger<ExecutionProcessor<IExecutionAdapter>>>();

            var execProcessor = new ExecutionProcessor<IExecutionAdapter>(
                mockExecAdapter.Object,
                mockLogger.Object,
                defaultProcessorOptions);

            Func<Task> act = async () => await execProcessor.ProcessRequestAsync(null, CancellationToken.None);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task ProcessRequestAsync_ValidExecutionRequest_ShouldUpdateExecutionContext()
        {
            var mockExecAdapter = new Mock<IExecutionAdapter>();
            var mockLogger = new Mock<ILogger<ExecutionProcessor<IExecutionAdapter>>>();

            var execProcessor = new ExecutionProcessor<IExecutionAdapter>(
                mockExecAdapter.Object,
                mockLogger.Object,
                defaultProcessorOptions);

            var execRequest = CreateDefaultExecutionRequest();
            var execContext = execRequest.ToExecutionContext().UpdateStatus(ExecutionStatus.Succeeded);

            mockExecAdapter.Setup(ea => ea.ExecuteAsync(execRequest, It.IsAny<CancellationToken>()))
                           .Returns(Task.FromResult(execContext));

            var newExecContext = await execProcessor.ProcessRequestAsync(execRequest, CancellationToken.None);

            newExecContext.Should().NotBeNull();
            newExecContext.ExecutionId.Should().Be(execRequest.ExecutionId);
            newExecContext.Status.Should().Be(ExecutionStatus.Succeeded);
        }

        [Fact]
        public async Task ProcessRequestAsync_ValidExecutionRequest_ShouldInvokeExecutionAdapter()
        {
            var mockExecAdapter = new Mock<IExecutionAdapter>();
            var mockLogger = new Mock<ILogger<ExecutionProcessor<IExecutionAdapter>>>();

            var execProcessor = new ExecutionProcessor<IExecutionAdapter>(
                mockExecAdapter.Object,
                mockLogger.Object,
                defaultProcessorOptions);

            var execRequest = CreateDefaultExecutionRequest();
            var execContext = execRequest.ToExecutionContext().UpdateStatus(ExecutionStatus.Succeeded);

            mockExecAdapter.Setup(ea => ea.ExecuteAsync(execRequest, It.IsAny<CancellationToken>()))
                           .Returns(Task.FromResult(execContext));

            await execProcessor.ProcessRequestAsync(execRequest, CancellationToken.None);

            mockExecAdapter.Verify(ea => ea.ExecuteAsync(execRequest, It.IsAny<CancellationToken>()));
        }

        private IExecutionProcessorOptions CreateDefaultProcessorOptions() =>
            new ExecutionProcessorOptions<ExecutionProcessor<IExecutionAdapter>>
            {
                DefaultExecutionTimeoutDuration = TimeSpan.FromHours(1)
            };

        private ExecutionRequest CreateDefaultExecutionRequest() =>
            new ExecutionRequest
            {
                ExecutionId = Guid.NewGuid().ToString()
            };
    }
}
