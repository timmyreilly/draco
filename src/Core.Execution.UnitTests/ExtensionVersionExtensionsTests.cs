using Draco.Core.Execution.Extensions;
using Draco.Core.Models;
using FluentAssertions;
using System;
using Xunit;

namespace Draco.Core.Execution.UnitTests
{
    public class ExtensionVersionExtensionsTests
    {
        [Fact]
        public void CreateNewExecutionId_ValidExtensionVersionAndTenantId_ShouldHaveCorrectPrefix()
        {
            var extensionVersion = new ExtensionVersion
            {
                ExtensionId = Guid.NewGuid().ToString(),
                ExtensionVersionId = Guid.NewGuid().ToString()
            };

            var tenantId = Guid.NewGuid().ToString();
            var expectedPrefix = $"{extensionVersion.ExtensionId}_{extensionVersion.ExtensionVersionId}_{tenantId}_";
            var executionId = extensionVersion.CreateNewExecutionId(tenantId);

            executionId.Should().StartWith(expectedPrefix);
        }
    }
}
