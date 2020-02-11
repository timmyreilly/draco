namespace Draco.Azure.Models.Cosmos.Extensions
{
    public static class ExecutionProfileExtensions
    {
        public static Core.Models.ExecutionProfile ToCoreModel(this ExecutionProfile cosmosModel, string profileName) =>
            new Core.Models.ExecutionProfile
            {
                BaseExecutionCost = cosmosModel.BaseExecutionCost,
                ClientConfiguration = cosmosModel.ClientConfiguration,
                ExecutionModelName = cosmosModel.ExecutionModelName,
                ExtensionSettings = cosmosModel.ExtensionSettings,
                IsActive = cosmosModel.IsActive,
                ProfileDescription = cosmosModel.ProfileDescription,
                ObjectProviderName = cosmosModel.ObjectProviderName,
                ProfileName = profileName,
                SupportedPriorities = cosmosModel.SupportedPriorities,
                ExecutionMode = cosmosModel.ExecutionMode,
                DirectExecutionTokenDuration = cosmosModel.DirectExecutionTokenDuration
            };

        public static ExecutionProfile ToCosmosModel(this Core.Models.ExecutionProfile coreModel) =>
            new ExecutionProfile
            {
                BaseExecutionCost = coreModel.BaseExecutionCost,
                ClientConfiguration = coreModel.ClientConfiguration,
                ExecutionModelName = coreModel.ExecutionModelName,
                ExtensionSettings = coreModel.ExtensionSettings,
                IsActive = coreModel.IsActive,
                ProfileDescription = coreModel.ProfileDescription,
                ObjectProviderName = coreModel.ObjectProviderName,
                SupportedPriorities = coreModel.SupportedPriorities,
                ExecutionMode = coreModel.ExecutionMode,
                DirectExecutionTokenDuration = coreModel.DirectExecutionTokenDuration
            };
    }
}
