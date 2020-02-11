namespace Draco.Azure.Models.Cosmos.Extensions
{
    public static class ExecutionExtensions
    {
        public static Core.Models.Execution ToCoreModel(this Execution cosmosModel) => new Core.Models.Execution
        {
            CreatedDateTimeUtc = cosmosModel.CreatedDateTimeUtc,
            ExecutionId = cosmosModel.ExecutionId,
            Executor = cosmosModel.Executor,
            ExecutionTimeoutDateTimeUtc = cosmosModel.ExpiresDateTimeUtc,
            ExtensionId = cosmosModel.ExtensionId,
            ExtensionVersionId = cosmosModel.ExtensionVersionId,
            LastUpdatedDateTimeUtc = cosmosModel.LastUpdatedDateTimeUtc,
            PercentComplete = cosmosModel.PercentageComplete,
            Priority = cosmosModel.Priority,
            Status = cosmosModel.Status,
            StatusMessage = cosmosModel.StatusMessage,
            StatusUpdateKey = cosmosModel.StatusUpdateKey,
            ValidateOnly = cosmosModel.ValidateOnly,
            ExecutionProfileName = cosmosModel.ExecutionProfileName,
            ProvidedInputObjects = cosmosModel.ProvidedInputObjects,
            ProvidedOutputObjects = cosmosModel.ProvidedOutputObjects,
            ExecutionModelName = cosmosModel.ExecutionModelName,
            ObjectProviderName = cosmosModel.ObjectProviderName,
            InputObjects = cosmosModel.InputObjects,
            OutputObjects = cosmosModel.OutputObjects,
            RequestData = cosmosModel.RequestData,
            ResultData = cosmosModel.ResultData,
            Mode = cosmosModel.Mode,
            SignatureRsaKeyXml = cosmosModel.SignatureRsaKeyXml,
            ExecutorProperties = cosmosModel.ExecutorProperties
        };

        public static Execution ToCosmosModel(this Core.Models.Execution coreModel) => new Execution
        {
            CreatedDateTimeUtc = coreModel.CreatedDateTimeUtc,
            ExecutionId = coreModel.ExecutionId,
            Executor = coreModel.Executor,
            ExpiresDateTimeUtc = coreModel.ExecutionTimeoutDateTimeUtc,
            ExtensionId = coreModel.ExtensionId,
            ExtensionVersionId = coreModel.ExtensionVersionId,
            LastUpdatedDateTimeUtc = coreModel.LastUpdatedDateTimeUtc,
            PercentageComplete = coreModel.PercentComplete,
            Priority = coreModel.Priority,
            Status = coreModel.Status,
            StatusMessage = coreModel.StatusMessage,
            StatusUpdateKey = coreModel.StatusUpdateKey,
            ValidateOnly = coreModel.ValidateOnly,
            ExecutionModelName = coreModel.ExecutionModelName,
            ExecutionProfileName = coreModel.ExecutionProfileName,
            ObjectProviderName = coreModel.ObjectProviderName,
            ProvidedInputObjects = coreModel.ProvidedInputObjects,
            ProvidedOutputObjects = coreModel.ProvidedOutputObjects,
            InputObjects = coreModel.InputObjects,
            OutputObjects = coreModel.OutputObjects,
            RequestData = coreModel.RequestData,
            ResultData = coreModel.ResultData,
            Mode = coreModel.Mode,
            SignatureRsaKeyXml = coreModel.SignatureRsaKeyXml,
            ExecutorProperties = coreModel.ExecutorProperties
        };
    }
}
