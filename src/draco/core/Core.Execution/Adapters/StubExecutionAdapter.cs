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
    public class StubExecutionAdapter : IExecutionAdapter // stub/v1: Provides default [successful] response.
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

            return Task.FromResult(request.ToExecutionContext().UpdateStatus(ExecutionStatus.Succeeded));
        }
    }
}
