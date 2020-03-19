// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.Execution.Api.Models;

namespace Draco.Execution.Api.Extensions
{
    /// <summary>
    /// Extension methods for converting to/from validation error core/API models
    /// </summary>
    public static class ValidationErrorApiModelExtensions
    {
        /// <summary>
        /// Converts a validation error API model to a core model
        /// </summary>
        /// <param name="apiModel"></param>
        /// <returns></returns>
        public static ExecutionValidationError ToCoreModel(this ValidationErrorApiModel apiModel) =>
            new ExecutionValidationError
            {
                ErrorCode = apiModel.ErrorCode,
                ErrorData = apiModel.ErrorData,
                ErrorId = apiModel.ErrorId,
                ErrorMessage = apiModel.ErrorMessage
            };

        /// <summary>
        /// Converts a validation error core model to an API model
        /// </summary>
        /// <param name="coreModel"></param>
        /// <returns></returns>
        public static ValidationErrorApiModel ToApiModel(this ExecutionValidationError coreModel) =>
            new ValidationErrorApiModel
            {
                ErrorCode = coreModel.ErrorCode,
                ErrorData = coreModel.ErrorData,
                ErrorId = coreModel.ErrorId,
                ErrorMessage = coreModel.ErrorMessage
            };
    }
}
