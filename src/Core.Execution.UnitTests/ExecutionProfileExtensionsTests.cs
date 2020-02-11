using Draco.Core.Execution.Constants;
using Draco.Core.Execution.Extensions;
using Draco.Core.Models;
using FluentAssertions;
using System;
using Xunit;

namespace Draco.Core.Execution.UnitTests
{
    public class ExecutionProfileExtensionsTests
    {
        [Fact]
        public void IsDefaultProfile_NullProfile_ShouldThrowException()
        {
            Action act = () => (null as ExecutionProfile).IsDefaultProfile();

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsDefaultProfile_ProfileWithDefaultName_ShouldBeTrue()
        {
            var defaultProfile = new ExecutionProfile { ProfileName = ExecutionProfiles.Default };
            var isDefaultProfile = defaultProfile.IsDefaultProfile();

            isDefaultProfile.Should().BeTrue();
        }

        [Fact]
        public void IsDefaultProfile_ProfileWithoutDefaultName_ShouldBeFalse()
        {
            var notDefaultProfile = new ExecutionProfile { ProfileName = "Not Default Profile" };
            var isDefaultProfile = notDefaultProfile.IsDefaultProfile();

            isDefaultProfile.Should().BeFalse();
        }
    }
}
