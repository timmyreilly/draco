// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;

namespace Draco.Api.InternalModels.Extensions
{
    /// <summary>
    /// Extension methods for converting to/from execution validation error API/core models
    /// </summary>
    public static class ValidationErrorExtensions
    {
        /// <summary>
        /// Converts an execution validation error core model to an API model
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

        /// <summary>
        /// Converts an execution validation error API model to a core model
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
    }
}
