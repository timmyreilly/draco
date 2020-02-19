// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentAssertions;
using System;
using Xunit;

namespace Draco.Core.Models.UnitTests
{
    public class ExtensionVersionTests
    {
        [Fact]
        public void ToString_GivenExtensionVersion_ShouldReturnExtensionAndExtensionVersionIds()
        {
            var extensionVersion = new ExtensionVersion
            {
                ExtensionId = Guid.NewGuid().ToString(),
                ExtensionVersionId = Guid.NewGuid().ToString()
            };

            var toString = extensionVersion.ToString();

            toString.Should().Be($"{extensionVersion.ExtensionId}_{extensionVersion.ExtensionVersionId}");
        }
    }
}
