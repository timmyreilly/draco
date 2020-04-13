// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Constants;
using Draco.Core.Models;
using System;

namespace Draco.Core.Execution.Extensions
{
    public static class ExecutionProfileExtensions
    {
        /// <summary>
        /// Checks to see whether or not an execution profile is the default one.
        /// </summary>
        /// <param name="execProfile">The execution profile to check</param>
        /// <returns></returns>
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
