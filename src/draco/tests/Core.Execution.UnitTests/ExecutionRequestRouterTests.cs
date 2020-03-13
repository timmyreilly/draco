// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Interfaces;
using Draco.Core.Execution.Processors;
using Draco.Core.Factories;
using Draco.Core.Models;
using Draco.Core.Models.Enumerations;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Draco.Core.Execution.UnitTests
{
    public class ExecutionRequestRouterTests
    {
        [Fact]
        public void RouteRequestAsync_NullExecutionRequest_ShouldThrowException()
        {
            var mockServiceProvider = new Mock<IServiceProvider>();
            var processorFactory = new NamedServiceFactory<IExecutionProcessor>();
            var execRequestRouter = new ExecutionRequestRouter(processorFactory, mockServiceProvider.Object);

            Func<Task> act = async () => await execRequestRouter.RouteRequestAsync(null, CancellationToken.None);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RouteRequestAsync_NoServiceWithGivenExecutionModelName_ShouldThrowException()
        {
            const string actualModelName = "actual-model/v1";
            const string expectedModelName = "expected-model/v1";

            var mockProcessor = new Mock<IExecutionProcessor>();
            var mockServiceProvider = new Mock<IServiceProvider>();

            var processorFactory = new NamedServiceFactory<IExecutionProcessor>(
                new Dictionary<string, Func<IServiceProvider, IExecutionProcessor>>
                {
                    [expectedModelName] = sp => mockProcessor.Object
                });

            var execRequest = new ExecutionRequest
            {
                ExecutionId = Guid.NewGuid().ToString(),
                ExecutionModelName = actualModelName
            };

            var execRequestRouter = new ExecutionRequestRouter(processorFactory, mockServiceProvider.Object);

            Func<Task> act = async () => await execRequestRouter.RouteRequestAsync(execRequest, CancellationToken.None);

            act.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public async Task RouteRequestAsync_ServiceWithGivenExecutionModelName_ShouldRouteSuccessfully()
        {
            const string modelName = "expected-model/v1";

            var mockProcessor = new Mock<IExecutionProcessor>();
            var mockServiceProvider = new Mock<IServiceProvider>();

            var processorFactory = new NamedServiceFactory<IExecutionProcessor>(
                new Dictionary<string, Func<IServiceProvider, IExecutionProcessor>>
                {
                    [modelName] = sp => mockProcessor.Object
                });

            var execRequest = new ExecutionRequest
            {
                ExecutionId = Guid.NewGuid().ToString(),
                ExecutionModelName = modelName
            };

            var execContext = new Core.Models.ExecutionContext
            {
                ExecutionId = execRequest.ExecutionId,
                Status = ExecutionStatus.Succeeded
            };

            mockProcessor.Setup(ep => ep.ProcessRequestAsync(execRequest, CancellationToken.None))
                         .Returns(Task.FromResult(execContext));

            var execRequestRouter = new ExecutionRequestRouter(processorFactory, mockServiceProvider.Object);
            var actualExecContext = await execRequestRouter.RouteRequestAsync(execRequest, CancellationToken.None);

            actualExecContext.Should().NotBeNull();
            actualExecContext.Should().Be(execContext);
        }
    }
}
