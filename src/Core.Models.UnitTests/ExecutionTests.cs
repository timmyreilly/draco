// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentAssertions;
using System;
using Xunit;

namespace Draco.Core.Models.UnitTests
{
    public class ExecutionTests
    {
        [Fact]
        public void ToString_GivenExecution_ShouldReturnExecutionId()
        {
            var execution = new Execution { ExecutionId = Guid.NewGuid().ToString() };
            var toString = execution.ToString();

            toString.Should().Be(execution.ExecutionId);
        }
    }
}
