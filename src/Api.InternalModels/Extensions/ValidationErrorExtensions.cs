using Draco.Core.Models;

namespace Draco.Api.InternalModels.Extensions
{
    public static class ValidationErrorExtensions
    {
        public static ValidationErrorApiModel ToApiModel(this ExecutionValidationError coreModel) =>
            new ValidationErrorApiModel
            {
                ErrorCode = coreModel.ErrorCode,
                ErrorData = coreModel.ErrorData,
                ErrorId = coreModel.ErrorId,
                ErrorMessage = coreModel.ErrorMessage
            };

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
