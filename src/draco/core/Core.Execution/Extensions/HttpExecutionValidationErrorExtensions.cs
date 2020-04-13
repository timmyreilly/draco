// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Execution.Models;
using Draco.Core.Models;

namespace Draco.Core.Execution.Extensions
{
    public static class HttpExecutionValidationErrorExtensions
    {
        /// <summary>
        /// Converts an HTTP execution validation error model to a core execution validation error model.
        /// </summary>
        /// <param name="httpModel">The HTTP execution validation error model</param>
        /// <returns></returns>
        public static ExecutionValidationError ToCoreModel(this HttpExecutionValidationError httpModel) =>
            new ExecutionValidationError
            {
                ErrorCode = httpModel.ErrorCode,
                ErrorData = httpModel.ErrorData,
                ErrorId = httpModel.ErrorId,
                ErrorMessage = httpModel.ErrorMessage
            };
    }
}
