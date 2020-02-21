// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Extensions;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Draco.Core.UnitTests
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void None_EmptyEnumerable_ShouldReturnTrue()
        {
            var list = new List<string>();
            var result = list.None();

            result.Should().BeTrue();
        }

        [Fact]
        public void None_NotEmptyEnumerable_ShouldReturnFalse()
        {
            var list = new List<string> { "Some text." };
            var result = list.None();

            result.Should().BeFalse();
        }

        [Fact]
        public void None_EnumerableWithUnmatchedPredicate_ShouldReturnTrue()
        {
            var list = new List<string> { "Some text." };
            var result = list.None(t => (t == "Some other text."));

            result.Should().BeTrue();
        }

        [Fact]
        public void None_EnumerableWithMatchedPredicate_ShouldReturnFalse()
        {
            const string someText = "Some text.";

            var list = new List<string> { someText };
            var result = list.None(t => (t == someText));

            result.Should().BeFalse();
        }
    }
}
