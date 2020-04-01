# Contributing

This project welcomes contributions and suggestions. Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the Microsoft Open Source Code of Conduct. For more information see the Code of Conduct FAQ or contact opencode@microsoft.com with any additional questions or comments.

## Azure naming conventions

The following sections outline the Azure naming conventions that Draco recommends contributors adhere to.

### Azure resource groups

The following table outlines the [resource group](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/overview#resource-groups) naming guidelines that Draco adheres to.

| Group type | Name | Description |
| ---------- | ---- | ----------- |
| [Common](/src/draco/infra/ArmTemplate/common/common-deploy.json) | `draco-common-rg` | Contains all the common resources (Azure container registry, Key vault, etc.) shared across multiple Draco platform deployments and other related Azure-based workloads.
| [Platform](/src/draco/infra/ArmTemplate/exthub/exthub-deploy.json) | `draco-platform-rg` | Contains all the resources (AKS cluster, Service bus namespace, etc.) used by independent Draco platform deployments. Note that these deployments depend on the resources deployed to the `draco-common-rg`.

### Azure resources

By default, the [Azure Resource Manager (ARM) templates](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/overview) included with Draco implement the following naming conventions for Azure resources. Both the [`common`](/src/draco/infra/ArmTemplate/common/common-deploy.json) and [`platform`](/src/draco/infra/ArmTemplate/exthub/exthub-deploy.json) ARM templates generate unique IDs (e.g., `draco-6eqa2ltcvlhp2`) that are deterministically based on the ID of the target resource group. We create this unique ID specifically to prevent DNS collisions with other Draco deployments. Both ARM templates return this unique ID through the `deploymentPrefix` [output parameter](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-syntax#outputs).

The following table outlines the resource naming conventions that Draco adheres to.

| Resource type | Suffix | Example |
| ------------- | ------ | ------- |
| Azure Kubernetes Service (AKS) cluster | `aks` | `draco-6eqa2ltcvlhp2-aks` |
| Service Bus namespace | `bus` | `draco-6eqa2ltcvlhp2-bus` |
| Event Grid topic | `egt-[topic name]` | `draco-6eqa2ltcvlhp2-egt-execution` |
| Storage account | `stor` | `draco6eqa2ltcvlhp2stor` |
| Search service | `srch` | `draco-6eqa2ltcvlhp2-srch` |
| Cosmos DB account | `cdb` | `draco-6eqa2ltcvlhp2-cdb` |
| API Management service | `apim` | `draco6eqa2ltcvlhp2apim` |
| Container registry | `acr` | `draco6eqa2ltcvlhp2acr` |
| Key vault | `akv` | `draco-6eqa2ltcvlhp2-akv` |
| Application insights | `appi` | `draco6eqa2ltcvlhp2appi`
