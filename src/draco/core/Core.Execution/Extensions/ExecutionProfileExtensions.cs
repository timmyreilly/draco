// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Constants;
using Draco.Core.Models;
using System;

namespace Draco.Core.Execution.Extensions
{
    public static class ExecutionProfileExtensions
    {
        public static bool IsDefaultProfile(this ExecutionProfile execProfile)
        {
            if (execProfile == null)
            {
                throw new ArgumentNullException(nameof(execProfile));
            }

            return (execProfile.ProfileName == ExecutionProfiles.Default);
        }
    }
}
