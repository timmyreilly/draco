// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Interfaces;
using Draco.Core.Models;
using Draco.Core.Models.Enumerations;
using Draco.Core.Models.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Execution.Adapters
{
    /// <summary>
    /// This execution adapter doesn't actually do anything.
    /// It's here simply for dev/test purposes and should never be used in production.
    /// </summary>
    public class StubExecutionAdapter : IExecutionAdapter
    {
        public Task<Draco.Core.Models.ExecutionContext> ExecuteAsync(ExecutionRequest request, CancellationToken cancelToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (cancelToken == null)
            {
                throw new ArgumentNullException(nameof(cancelToken));
            }

            // We're not actually doing anything. Mark the execution as [Succeeded] and move on...

            return Task.FromResult(request.ToExecutionContext().UpdateStatus(ExecutionStatus.Succeeded));
        }
    }
}
