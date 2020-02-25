// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using FluentAssertions;
using System;

namespace Draco.Core.Models.UnitTests
{
    public class ExtensionTests
    {
        public void ToString_GivenExtension_ShouldReturnExtensionId()
        {
            var extension = new Extension { ExtensionId = Guid.NewGuid().ToString() };
            var toString = extension.ToString();

            toString.Should().Be(extension.ExtensionId);
        }
    }
}
