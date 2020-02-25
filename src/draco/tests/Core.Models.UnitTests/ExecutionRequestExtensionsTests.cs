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
    public class ExecutionRequestExtensionsTests
    {
        [Fact]
        public void ToExecutionContext_GivenExecutionRequest_ShouldReturnEquivalentExecutionContext()
        {
            var execRequest = new ExecutionRequest
            {
                CreatedDateTimeUtc = DateTime.UtcNow.AddMinutes(-10),
                ExecutionProfileName = "DefaultExecutionProfile",
                ExecutionId = Guid.NewGuid().ToString(),
                ExtensionId = Guid.NewGuid().ToString(),
                ExtensionVersionId = Guid.NewGuid().ToString(),
                LastUpdatedDateTimeUtc = DateTime.UtcNow.AddMinutes(-5),
                Priority = ExecutionPriority.High,
                StatusUpdateKey = Guid.NewGuid().ToString(),
                ExecutionTimeoutDateTimeUtc = DateTime.UtcNow.AddHours(1),
                ExecutionModelName = "DefaultExecutionModel",
                ObjectProviderName = "DefaultObjectProvider",
                SupportedServices = new Dictionary<string, JObject> { ["ServiceA"] = null },
                ProvidedInputObjects = new List<string> { "InputObjectA" },
                InputObjects = new Dictionary<string, ExtensionInputObject> { ["InputObjectA"] = null },
                OutputObjects = new Dictionary<string, ExtensionOutputObject> { ["OutputObjectA"] = null }
            };

            var execContext = execRequest.ToExecutionContext();

            execContext.Should().NotBeNull();
            execContext.CreatedDateTimeUtc.Should().Be(execRequest.CreatedDateTimeUtc);
            execContext.ExecutionProfileName.Should().Be(execRequest.ExecutionProfileName);
            execContext.ExecutionId.Should().Be(execRequest.ExecutionId);
            execContext.ExtensionId.Should().Be(execRequest.ExtensionId);
            execContext.ExtensionVersionId.Should().Be(execRequest.ExtensionVersionId);
            execContext.LastUpdatedDateTimeUtc.Should().Be(execRequest.LastUpdatedDateTimeUtc);
            execContext.Priority.Should().Be(execRequest.Priority);
            execContext.StatusUpdateKey.Should().Be(execRequest.StatusUpdateKey);
            execContext.ExecutionTimeoutDateTimeUtc.Should().Be(execRequest.ExecutionTimeoutDateTimeUtc);
            execContext.ExecutionModelName.Should().Be(execRequest.ExecutionModelName);
            execContext.ObjectProviderName.Should().Be(execRequest.ObjectProviderName);
            execContext.SupportedServices.Should().BeEquivalentTo(execRequest.SupportedServices);
            execContext.ProvidedInputObjects.Should().BeEquivalentTo(execRequest.ProvidedInputObjects);
            execContext.InputObjects.Should().BeEquivalentTo(execRequest.InputObjects);
            execContext.OutputObjects.Should().BeEquivalentTo(execRequest.OutputObjects);
        }

        [Fact]
        public void CalculateExecutionTimeoutDateTimeUtc_GivenNullExecutionRequest_ShouldThrowException()
        {
            Action act = () => (null as ExecutionRequest).CalculateExecutionTimeoutDateTimeUtc(TimeSpan.FromHours(2));

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CalculateExecutionTimeoutDateTimeUtc_GivenExecutionRequestWithTimeoutDuration_ShouldReturnDateTimeBasedOnRequestTimeoutDuration()
        {
            var execRequest = new ExecutionRequest
            {
                ExecutionId = Guid.NewGuid().ToString(),
                ExecutionTimeoutDuration = TimeSpan.FromHours(1)
            };

            execRequest.CalculateExecutionTimeoutDateTimeUtc(TimeSpan.FromHours(2));

            execRequest.ExecutionTimeoutDateTimeUtc.Should().BeAfter(DateTime.UtcNow.AddHours(1).AddSeconds(-5)).And
                                                            .BeBefore(DateTime.UtcNow.AddHours(1).AddSeconds(5));
        }

        [Fact]
        public void CalculateExecutionTimeoutDateTimeUtc_GivenExecutionRequestWithoutTimeoutDuration_ShouldReturnDateTimeBasedOnDefaultTimeoutDuration()
        {
            var execRequest = new ExecutionRequest { ExecutionId = Guid.NewGuid().ToString() };

            execRequest.CalculateExecutionTimeoutDateTimeUtc(TimeSpan.FromHours(2));

            execRequest.ExecutionTimeoutDateTimeUtc.Should().BeAfter(DateTime.UtcNow.AddHours(2).AddSeconds(-5)).And
                                                            .BeBefore(DateTime.UtcNow.AddHours(2).AddSeconds(5));
        }
    }
}
