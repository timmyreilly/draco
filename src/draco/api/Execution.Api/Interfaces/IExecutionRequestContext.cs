// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using System.Collections.Generic;

namespace Draco.Execution.Api.Interfaces
{
    /// <summary>
    /// This is a convenience interface used to internally (within the execution API) pass around all the information needed
    /// to process an execution request. 
    /// </summary>
    public interface IExecutionRequestContext
    {
         Core.Models.Execution Execution { get; set; }

         Extension Extension { get; set; }

         ExtensionVersion ExtensionVersion { get; set; }

         ExecutionProfile ExecutionProfile { get; set; }

         List<string> ValidationErrors { get; set; }
    }
}
