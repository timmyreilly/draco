// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Draco.Core.Models;
using Draco.Execution.Api.Models;

namespace Draco.Execution.Api.Extensions
{
    public static class ValidationErrorApiModelExtensions
    {
        public static ExecutionValidationError ToCoreModel(this ValidationErrorApiModel apiModel) =>
            new ExecutionValidationError
            {
                ErrorCode = apiModel.ErrorCode,
                ErrorData = apiModel.ErrorData,
                ErrorId = apiModel.ErrorId,
                ErrorMessage = apiModel.ErrorMessage
            };

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
