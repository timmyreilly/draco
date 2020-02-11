using Draco.Core.Models.Enumerations;
using System;

namespace Draco.Core.Models.Extensions
{
    public static class ExecutionContextExtensions
    {
        public static ExecutionUpdate ToExecutionUpdate(this ExecutionContext execContext) =>
            new ExecutionUpdate
            {
                PercentComplete = execContext.PercentComplete,
                Status = execContext.Status.ToString(),
                StatusMessage = execContext.StatusMessage,
                ResultData = execContext.ResultData,
                ValidationErrors = execContext.ValidationErrors,
                ProvidedOutputObjects = execContext.ProvidedOutputObjects,
                StatusUpdateKey = execContext.StatusUpdateKey,
                LastUpdatedDateTimeUtc = execContext.LastUpdatedDateTimeUtc,
                ExecutionTimeoutDateTimeUtc = execContext.ExecutionTimeoutDateTimeUtc,
                ExecutorProperties = execContext.ExecutorProperties
            };

        public static ExecutionContext UpdateStatus(this ExecutionContext execContext, ExecutionStatus execStatus)
        {
            if (execContext == null)
            {
                throw new ArgumentNullException(nameof(execContext));
            }

            if (execContext.Status != execStatus)
            {
                execContext.Status = execStatus;
                execContext.LastUpdatedDateTimeUtc = DateTime.UtcNow;
            }

            return execContext;
        }
    }
}
