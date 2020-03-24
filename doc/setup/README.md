# Setup the Draco platform components

The following instructions are how to build and setup the Draco platform components.  These components are required to build an extension, but many coporate developers will probably be given an environment to use (check with your lead).

See [Draco Architecture](../architecture/azure-architecture.md) for more information.

> The following instructions have been tested and verified using Ubuntu 18.04 and MacOS Catalina 10.15

## Pre-requisites

* Azure Subscription
* Permission to create a service principal (SPN) in Azure AD
* [Kubernetes CLI](https://kubernetes.io/docs/tasks/tools/install-kubectl/)
* [Docker](https://www.docker.com/products/docker-desktop)
* [Helm](https://github.com/helm/helm/releases)
* [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
* [Azure Kubernetes CLI / kubectl](https://docs.microsoft.com/en-us/cli/azure/aks?view=azure-cli-latest#az-aks-install-cli)

## Specific to MacOS

* [Home Brew](https://brew.sh/) to install the following tools:
  * azure-cli (alternative install method to direct download)
  * docker  (You still need Docker Desktop from above)
  * kubernetes-cli
  * helm

## Initialize variables

The variable values are used in the following setup commands.  You can change the values to your preference for your environment.

> NOTE: The value for DRACO_SPN_K8S needs to be unique to the Azure AD tenant protecting your subscription.  If you have other deployments of this platform in subscriptions protected by the same Azure AD, then change the value to something that doesn't currently exist in your Azure AD tenant.

```bash
DRACO_COMMON_RG_NAME="draco-common-rg"
DRACO_EXTHUB_RG_NAME="draco-exthub-rg"
DRACO_REGION="eastus2"
DRACO_SPN_K8S="http://draco-k8s-***UNIQUE*VALUE***" 
DRACO_SPN_KEY_NAME="draco-aks-spn" 
```

## Login to Azure Subscription

> NOTE: This may launch a browser to validate your credentials.

```bash
az login
```

### Ensure correct Azure subscription is in use

If you have more than one Azure subscription make sure you choose the correct subscription before running any of these commands.

```bash
az account list  # Return list of accounts
az account set --subscription "Name of Subscription"
```

## Setup Draco common infrastructure

Draco leverages Azure resources that generally should be managed in a separate resource group.  Specifically, Draco uses Azure Contrainer Registry (ACR) to store container images and Azure Key Vault to store secrets.  These resources are deployed in a separate _common_ resource group so you can use them for other workloads beyond just Draco.

```bash
cd src/draco
az group create --location $DRACO_REGION --name $DRACO_COMMON_RG_NAME
ACR_RESOURCE_ID=$(az group deployment create --resource-group $DRACO_COMMON_RG_NAME --template-file ./infra/ArmTemplate/common/common-deploy.json --query properties.outputs.acrResourceId.value --out tsv)

ACR_NAME=$(az group deployment show --resource-group $DRACO_COMMON_RG_NAME --name common-deploy --query properties.outputs.acrName.value --output tsv)
AKV_NAME=$(az group deployment show --resource-group $DRACO_COMMON_RG_NAME --name common-deploy --query properties.outputs.kvName.value --output tsv)

SPN_PASSWORD=$(az ad sp create-for-rbac --name $DRACO_SPN_K8S --scopes $ACR_RESOURCE_ID --role acrpull --query password --output tsv)
SPN_APP_ID=$(az ad sp show --id $DRACO_SPN_K8S --query appId --output tsv)

UPN=$(az account show --query user.name --output tsv)
az keyvault set-policy --name $AKV_NAME --upn $UPN --secret-permissions get list set
az keyvault secret set --vault-name $AKV_NAME --name $DRACO_SPN_KEY_NAME --value $SPN_PASSWORD
```

> NOTE: To show the secret stored in keyvault, you can use:
```bash
az keyvault secret show --name $DRACO_SPN_KEY_NAME --vault-name $AKV_NAME --query value --output tsv
```


## Setup Draco platform infrastructure

The Draco platform is comprised of an Azure Kubernetes (AKS) cluster, Cosmos DB, Storage, and other resources specific to running the Draco platform.  This section deployes the Azure resources for the Draco platform and configures the AKS cluster with minimum permissions to pull container images from ACR.

```bash
# Get the latest stable version of AKS available in the region.
AKSK8SVERSION=$(az aks get-versions --location $DRACO_REGION --query "orchestrators[?orchestratorType=='Kubernetes'].orchestratorVersion | sort(@) | [-2:-1:]" --output tsv)

az group create --location $DRACO_REGION --name $DRACO_EXTHUB_RG_NAME
az group deployment create --resource-group $DRACO_EXTHUB_RG_NAME --template-file ./infra/ArmTemplate/exthub/exthub-deploy.json --parameters aksK8sVersion=$AKSK8SVERSION aksServicePrincipalClientId=$SPN_APP_ID aksServicePrincipalClientSecret=$SPN_PASSWORD
 ```

## Retrieve Draco service configuration settings

The Draco platform infrastructure deployment contains configuration outputs for each of the services running in AKS that make up the Draco platform.  This section saves these configuration outputs to a JSON file (application settings) for each service.

```bash
az group deployment show --resource-group $DRACO_EXTHUB_RG_NAME --name exthub-deploy --query properties.outputs.catalogApiConfiguration.value > appsettings-catalogapi.json
az group deployment show --resource-group $DRACO_EXTHUB_RG_NAME --name exthub-deploy --query properties.outputs.extensionMgmtApiConfiguration.value > appsettings-extensionmgmtapi.json
az group deployment show --resource-group $DRACO_EXTHUB_RG_NAME --name exthub-deploy --query properties.outputs.executionConsoleConfiguration.value > appsettings-execconsole.json
az group deployment show --resource-group $DRACO_EXTHUB_RG_NAME --name exthub-deploy --query properties.outputs.executionApiConfiguration.value > appsettings-execapi.json
```

## Upload Draco service configuration settings to blob storage

In this section, the application settings for the Draco service are uploaded to the blob storage account.  These are referenced later in the steps to deploy the container images into AKS.

```bash
STG_CONN_STR=$(az group deployment show --resource-group $DRACO_EXTHUB_RG_NAME --name exthub-deploy --query properties.outputs.executionApiConfiguration.value.platforms.azure.objectStorage.blobStorage.storageAccount.connectionString --output tsv)
az storage blob upload --file ./appsettings-catalogapi.json --connection-string $STG_CONN_STR --container-name configuration --name appsettings-catalogapi.json
az storage blob upload --file ./appsettings-execapi.json --connection-string $STG_CONN_STR --container-name configuration --name appsettings-execapi.json
az storage blob upload --file ./appsettings-execconsole.json --connection-string $STG_CONN_STR --container-name configuration --name appsettings-execconsole.json
az storage blob upload --file ./appsettings-extensionmgmtapi.json --connection-string $STG_CONN_STR --container-name configuration --name appsettings-extensionmgmtapi.json
```

## [Optional] Cleanup local appsettings*.json files

You can delete the local copy of these `appsettings-*.json` files after uploading them to blob storage in the previous section.

```bash
rm ./appsettings-catalogapi.json
rm ./appsettings-execapi.json
rm ./appsettings-execconsole.json
rm ./appsettings-extensionmgmtapi.json
```

## Configure Azure Cosmos DB

In this section, the Azure Search resource is configured with necessary index and data source definitions to provide Draco's catalog search capabilities.

```bash
az deployment group show --resource-group $DRACO_EXTHUB_RG_NAME --name exthub-deploy --query properties.outputs.azureSearchDataSourceConfiguration.value > azure-search-datasource-config.json
az deployment group show --resource-group $DRACO_EXTHUB_RG_NAME --name exthub-deploy --query properties.outputs.azureSearchIndexConfiguration.value > azure-search-index-config.json
az deployment group show --resource-group $DRACO_EXTHUB_RG_NAME --name exthub-deploy --query properties.outputs.azureSearchIndexerConfiguration.value > azure-search-indexer-config.json

# Get the admin-key for subsequent Azure Search REST API calls
SEARCH_SERVICE_NAME=$(az deployment group show --resource-group $DRACO_EXTHUB_RG_NAME --name exthub-deploy --query properties.outputs.azureSearchServiceName.value --output tsv)
COSMOS_ADMIN_KEY=$(az search admin-key show --resource-group $DRACO_EXTHUB_RG_NAME --service-name $SEARCH_SERVICE_NAME --query primaryKey --output tsv)

# Create Azure Search data source
curl -X POST https://$SEARCH_SERVICE_NAME.search.windows.net/datasources\?api-version=2019-05-06 -d @azure-search-datasource-config.json --header "Content-Type: application/json" --header "api-key: $COSMOS_ADMIN_KEY"

# Create Azure Search index
curl -X POST https://$SEARCH_SERVICE_NAME.search.windows.net/indexes\?api-version=2019-05-06 -d @azure-search-index-config.json --header "Content-Type: application/json" --header "api-key: $COSMOS_ADMIN_KEY"

# Create Azure Search indexer
curl -X POST https://$SEARCH_SERVICE_NAME.search.windows.net/indexers\?api-version=2019-05-06 -d @azure-search-indexer-config.json --header "Content-Type: application/json" --header "api-key: $COSMOS_ADMIN_KEY"

# Clean up local files
rm ./azure-search-datasource-config.json
rm ./azure-search-index-config.json
rm ./azure-search-indexer-config.json
```

## Configure kubectl to manage AKS

Configure your `kubectl` command-line tool so you can access and manage your AKS instance.

```bash
K8S_NAME=$(az resource list --resource-group $DRACO_EXTHUB_RG_NAME --resource-type Microsoft.ContainerService/managedClusters --query '[0].name' --output tsv)
az aks get-credentials --resource-group $DRACO_EXTHUB_RG_NAME --name $K8S_NAME
```

## Build container images

These steps use the `Dockerfile` for each of the Draco services to build container images and tags them so they can be stored in ACR.

```bash
docker build . --file ./api/Catalog.Api/Dockerfile --tag "$ACR_NAME.azurecr.io/xhub-catalogapi:latest"
docker build . --file ./api/Execution.Api/Dockerfile --tag "$ACR_NAME.azurecr.io/xhub-executionapi:latest"
docker build . --file ./api/ExtensionManagement.Api/Dockerfile --tag "$ACR_NAME.azurecr.io/xhub-extensionmgmtapi:latest"
docker build . --file ./api/ExecutionAdapter.Api/Dockerfile --tag "$ACR_NAME.azurecr.io/xhub-executionadapterapi:latest"
docker build . --file ./api/ExtensionService.Api/Dockerfile --tag "$ACR_NAME.azurecr.io/xhub-extensionserviceapi:latest"
docker build . --file ./api/ObjectStorageProvider.Api/Dockerfile --tag "$ACR_NAME.azurecr.io/xhub-objectproviderapi:latest"
docker build . --file ./core/Agent/ExecutionAdapter.ConsoleHost/Dockerfile --tag "$ACR_NAME.azurecr.io/xhub-executionconsole:latest"
```

## Push container images to ACR

```bash
az acr login --name $ACR_NAME
docker push "$ACR_NAME.azurecr.io/xhub-catalogapi"
docker push "$ACR_NAME.azurecr.io/xhub-executionapi"
docker push "$ACR_NAME.azurecr.io/xhub-extensionmgmtapi"
docker push "$ACR_NAME.azurecr.io/xhub-executionconsole"
docker push "$ACR_NAME.azurecr.io/xhub-executionadapterapi"
docker push "$ACR_NAME.azurecr.io/xhub-extensionserviceapi"
docker push "$ACR_NAME.azurecr.io/xhub-objectproviderapi"
```

## Deploy to AKS

Using helm and the charts in the `./helm` folder, deploy the Draco platform services into AKS.

```bash
cd helm
helm install initial extension-hubs --set configuration.storageConnectionString="$STG_CONN_STR" --set images.repository="$ACR_NAME.azurecr.io"
```

## Validate Draco is running in AKS

To validate you have the Draco platform running you can query the pods.  You will see 3 replicas of each of the services.  All replicas should be in the Running status.

```bash
kubectl get pods  
```

Your Draco platform components are now ready.  You should proceed to the instructions on how to build an extension, or deploy a sample extension.

> When ready to remove the Draco platform components please see the [Uninstall](UNINSTALL.md) document.
