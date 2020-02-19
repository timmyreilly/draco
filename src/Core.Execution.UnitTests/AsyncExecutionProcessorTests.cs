// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Interfaces;
using Draco.Core.Execution.Options;
using Draco.Core.Execution.Processors;
using Draco.Core.Interfaces;
using Draco.Core.Models;
using Draco.Core.Models.Enumerations;
using Draco.Core.Models.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Draco.Core.Execution.UnitTests
{
    public class AsyncExecutionProcessorTests
    {
        const string DefaultUpdateExecutionStatusUrl = "http://test.com/execution/status";

        readonly IExecutionProcessorOptions defaultProcessorOptions;

        public AsyncExecutionProcessorTests()
        {
            defaultProcessorOptions = CreateDefaultProcessorOptions();
        }

        [Fact]
        public void ProcessRequestAsync_NullExecutionRequest_ShouldThrowException()
        {
            var mockExecAdapter = new Mock<IExecutionAdapter>();
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockLogger = new Mock<ILogger<AsyncExecutionProcessor<IExecutionAdapter>>>();

            var asyncExecProcessor = new AsyncExecutionProcessor<IExecutionAdapter>(
                mockExecAdapter.Object,
                mockJsonHttpClient.Object,
                mockLogger.Object,
                defaultProcessorOptions);

            Func<Task> act = async () => await asyncExecProcessor.ProcessRequestAsync(null, CancellationToken.None);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task ProcessRequestAsync_ValidExecutionRequest_ShouldPutProcessingUpdateToExecutionApi()
        {
            var mockExecAdapter = new Mock<IExecutionAdapter>();
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockLogger = new Mock<ILogger<AsyncExecutionProcessor<IExecutionAdapter>>>();

            var asyncExecProcessor = new AsyncExecutionProcessor<IExecutionAdapter>(
                mockExecAdapter.Object,
                mockJsonHttpClient.Object,
                mockLogger.Object,
                defaultProcessorOptions);

            var execRequest = CreateDefaultExecutionRequest();
            var execContext = execRequest.ToExecutionContext().UpdateStatus(ExecutionStatus.Succeeded);

            mockExecAdapter.Setup(ea => ea.ExecuteAsync(execRequest, It.IsAny<CancellationToken>()))
                           .Returns(Task.FromResult(execContext));

            ExecutionUpdate execUpdate1 = null;
            ExecutionUpdate execUpdate2 = null;

            mockJsonHttpClient.Setup(hc => hc.PutAsync(execRequest.UpdateExecutionStatusUrl, It.IsAny<ExecutionUpdate>()))
                              .Returns(Task.FromResult(new HttpResponse(HttpStatusCode.OK)))
                              .Callback<string, object>((u, eu) =>
                              {
                                  if (execUpdate1 == null)
                                  {
                                      execUpdate1 = eu as ExecutionUpdate;
                                  }
                                  else
                                  {
                                      execUpdate2 = eu as ExecutionUpdate;
                                  }
                              });

            await asyncExecProcessor.ProcessRequestAsync(execRequest, CancellationToken.None);

            execUpdate1.Should().NotBeNull();
            execUpdate1.Status.Should().Be(ExecutionStatus.Processing.ToString());
        }

        [Fact]
        public async Task ProcessRequestAsync_ValidExecutionRequest_ShouldPutExecutionResultUpdateToExecutionApi()
        {
            var mockExecAdapter = new Mock<IExecutionAdapter>();
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockLogger = new Mock<ILogger<AsyncExecutionProcessor<IExecutionAdapter>>>();

            var asyncExecProcessor = new AsyncExecutionProcessor<IExecutionAdapter>(
                mockExecAdapter.Object,
                mockJsonHttpClient.Object,
                mockLogger.Object,
                defaultProcessorOptions);

            var execRequest = CreateDefaultExecutionRequest();
            var execContext = execRequest.ToExecutionContext().UpdateStatus(ExecutionStatus.Succeeded);

            mockExecAdapter.Setup(ea => ea.ExecuteAsync(execRequest, It.IsAny<CancellationToken>()))
                           .Returns(Task.FromResult(execContext));

            ExecutionUpdate execUpdate1 = null;
            ExecutionUpdate execUpdate2 = null;

            mockJsonHttpClient.Setup(hc => hc.PutAsync(execRequest.UpdateExecutionStatusUrl, It.IsAny<ExecutionUpdate>()))
                              .Returns(Task.FromResult(new HttpResponse(HttpStatusCode.OK)))
                              .Callback<string, object>((u, eu) =>
                              {
                                  if (execUpdate1 == null)
                                  {
                                      execUpdate1 = eu as ExecutionUpdate;
                                  }
                                  else
                                  {
                                      execUpdate2 = eu as ExecutionUpdate;
                                  }
                              });

            await asyncExecProcessor.ProcessRequestAsync(execRequest, CancellationToken.None);

            execUpdate2.Should().NotBeNull();
            execUpdate2.Status.Should().Be(ExecutionStatus.Succeeded.ToString());
        }

        [Fact]
        public async Task ProcessRequestAsync_ValidExecutionRequest_ShouldUpdateExecutionContext()
        {
            var mockExecAdapter = new Mock<IExecutionAdapter>();
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockLogger = new Mock<ILogger<AsyncExecutionProcessor<IExecutionAdapter>>>();

            var asyncExecProcessor = new AsyncExecutionProcessor<IExecutionAdapter>(
                mockExecAdapter.Object,
                mockJsonHttpClient.Object,
                mockLogger.Object,
                defaultProcessorOptions);

            var execRequest = CreateDefaultExecutionRequest();
            var execContext = execRequest.ToExecutionContext().UpdateStatus(ExecutionStatus.Succeeded);

            mockExecAdapter.Setup(ea => ea.ExecuteAsync(execRequest, It.IsAny<CancellationToken>()))
                           .Returns(Task.FromResult(execContext));

            mockJsonHttpClient.Setup(hc => hc.PutAsync(execRequest.UpdateExecutionStatusUrl, It.IsAny<ExecutionUpdate>()))
                              .Returns(Task.FromResult(new HttpResponse(HttpStatusCode.OK)));

            var newExecContext = await asyncExecProcessor.ProcessRequestAsync(execRequest, CancellationToken.None);

            newExecContext.Should().NotBeNull();
            newExecContext.ExecutionId.Should().Be(execRequest.ExecutionId);
            newExecContext.Status.Should().Be(ExecutionStatus.Succeeded);
        }

        [Fact]
        public async Task ProcessRequestAsync_ValidExecutionRequest_ShouldInvokeExecutionAdapter()
        {
            var mockExecAdapter = new Mock<IExecutionAdapter>();
            var mockJsonHttpClient = new Mock<IJsonHttpClient>();
            var mockLogger = new Mock<ILogger<AsyncExecutionProcessor<IExecutionAdapter>>>();

            var asyncExecProcessor = new AsyncExecutionProcessor<IExecutionAdapter>(
                mockExecAdapter.Object,
                mockJsonHttpClient.Object,
                mockLogger.Object,
                defaultProcessorOptions);

            var execRequest = CreateDefaultExecutionRequest();
            var execContext = execRequest.ToExecutionContext().UpdateStatus(ExecutionStatus.Succeeded);

            mockExecAdapter.Setup(ea => ea.ExecuteAsync(execRequest, It.IsAny<CancellationToken>()))
                           .Returns(Task.FromResult(execContext));

            mockJsonHttpClient.Setup(hc => hc.PutAsync(execRequest.UpdateExecutionStatusUrl, It.IsAny<ExecutionUpdate>()))
                              .Returns(Task.FromResult(new HttpResponse(HttpStatusCode.OK)));

            await asyncExecProcessor.ProcessRequestAsync(execRequest, CancellationToken.None);

            mockExecAdapter.Verify(ea => ea.ExecuteAsync(execRequest, It.IsAny<CancellationToken>()));
        }

        private IExecutionProcessorOptions CreateDefaultProcessorOptions() =>
            new ExecutionProcessorOptions<AsyncExecutionProcessor<IExecutionAdapter>>
            {
                DefaultExecutionTimeoutDuration = TimeSpan.FromHours(1)
            };

        private ExecutionRequest CreateDefaultExecutionRequest() =>
            new ExecutionRequest
            {
                ExecutionId = Guid.NewGuid().ToString(),
                UpdateExecutionStatusUrl = DefaultUpdateExecutionStatusUrl
            };
    }
}
