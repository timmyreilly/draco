#!/bin/bash

# This script sets up all the platform Draco components in Azure.
# For more information, see /doc/setup/README.md.

usage() { echo "Usage: $0 <-l azure-location> <-a aks-sp-appid> <-k common-akv-name> <-r common-acr-name> <-s subscription-id> [-c common-rg-name] [-p platform-rg-name]"; }

DRACO_DEPLOYMENT_NAME="deploy-platform-$(date -u +%m%d%Y-%H%M%S)"
DRACO_COMMON_RG_NAME="draco-common-rg"
DRACO_PLATFORM_RG_NAME="draco-platform-rg"
DRACO_SPN_AKV_KEY_NAME="draco-akv-spn";

while getopts "l:a:s:p:c:k:r:" opt; do
    case $opt in
        l)
            LOCATION=$OPTARG # [Requried] -- Azure location to deploy platform to.
        ;;
        a)
            AKS_SPN_APP_ID=$OPTARG # [Required] -- AKS service principal app ID. Created by ../common/setup-common.sh.
        ;;
        s)
            SUBSCRIPTION_ID=$OPTARG # [Required]
        ;;
        p)
            DRACO_PLATFORM_RG_NAME=$OPTARG # [Optional] -- If not provided, we default to "draco-platform-rg".
        ;;
        c)
            DRACO_COMMON_RG_NAME=$OPTARG # [Optional] -- If not provided, we default to "draco-common-rg".
        ;;
        k)
            AKV_NAME=$OPTARG # [Required] -- The name of the common key vault. Created by ../common/setup-common.sh.
        ;;
        r)
            ACR_NAME=$OPTARG # [Required] -- The name of the common ACR. Created by ../common/setup-common.sh.
        ;;
        \?)
            usage
            exit 1
        ;;
    esac
done

# Check to make sure we have all the arguments that we need...
[[ -z $LOCATION || -z $AKS_SPN_APP_ID || -z $ACR_NAME || -z $SUBSCRIPTION_ID ]] && { usage; exit 1; }

# Make sure we're logged into Azure...
[[ -z $(az account list --refresh --output tsv) ]] && az login

# Switch to the specified subscription...
az account set --subscription "$SUBSCRIPTION_ID"

# Check to make sure that the common resource group is there...
if [[ -z $(az group list --query "[?name=='$DRACO_COMMON_RG_NAME']" --output tsv) ]]; then
    echo -e "\nDraco common resource group [$DRACO_COMMON_RG_NAME] does not exist. Please setup common resources before continuing. See /doc/setup/README.md for more information."
    exit 1
else
    echo -e "\nDraco common resource group [$DRACO_COMMON_RG_NAME] already exists."
fi

# Create the platform resource group if it doesn't already exist...
if [[ -z $(az group list --query "[?name=='$DRACO_PLATFORM_RG_NAME']" --output tsv) ]]; then
    echo -e "\nCreating Draco platform resource group [$DRACO_PLATFORM_RG_NAME]...\n"
    az group create --location "$LOCATION" --name "$DRACO_PLATFORM_RG_NAME" --tags platform=draco  
else
    echo -e "\nDraco platform resource group [$DRACO_PLATFORM_RG_NAME] already exists."
fi

# Get the latest version of K8S available in the specified region...
LATEST_AKS_K8S_VERSION=$(az aks get-versions --location "$LOCATION" --query "orchestrators[?orchestratorType=='Kubernetes'].orchestratorVersion | sort(@) | [-2:-1:]" --output tsv)

echo -e "\nLatest stable AKS Kubernetes version in [$LOCATION] is [$LATEST_AKS_K8S_VERSION]."
echo -e "\nGetting AKS service principal [$AKS_SPN_APP_ID] password from key vault [$AKV_NAME]..."

# Get the ASK SPN object Id from AAD
AKS_SPN_OBJECTID=$(az ad sp show --id "$AKS_SPN_APP_ID" --query objectId --output tsv)

# Get AKS SPN password from key vault...
AKS_SPN_PASSWORD=$(az keyvault secret show --name "$DRACO_SPN_AKV_KEY_NAME" --vault-name "$AKV_NAME" --query value --output tsv)

# Stand everything up...
echo -e "\nDeploying Draco platform resources to resource group [$DRACO_PLATFORM_RG_NAME]. This can take up to 30 minutes...\n"

az deployment group create --verbose --resource-group "$DRACO_PLATFORM_RG_NAME" --name "$DRACO_DEPLOYMENT_NAME" --template-file "./exthub-deploy.json" --parameters aksK8sVersion="$LATEST_AKS_K8S_VERSION" aksServicePrincipalClientId="$AKS_SPN_APP_ID" aksServicePrincipalObjectId="$AKS_SPN_OBJECTID" aksServicePrincipalClientSecret="$AKS_SPN_PASSWORD"

DEPLOYMENT_PREFIX=$(az deployment group show --resource-group "$DRACO_PLATFORM_RG_NAME" --name "$DRACO_DEPLOYMENT_NAME" --query properties.outputs.deploymentPrefix.value --output tsv)

echo -e "\nDraco deployment [$DRACO_DEPLOYMENT_NAME] resource prefix is [$DEPLOYMENT_PREFIX]."

# Get Draco search configuration from ARM template output...
echo -e "\nGetting Draco search configuration from deployment [$DRACO_DEPLOYMENT_NAME]..."

az deployment group show --resource-group "$DRACO_PLATFORM_RG_NAME" --name "$DRACO_DEPLOYMENT_NAME" --query properties.outputs.azureSearchDataSourceConfiguration.value > azure-search-datasource-config.json
az deployment group show --resource-group "$DRACO_PLATFORM_RG_NAME" --name "$DRACO_DEPLOYMENT_NAME" --query properties.outputs.azureSearchIndexConfiguration.value > azure-search-index-config.json
az deployment group show --resource-group "$DRACO_PLATFORM_RG_NAME" --name "$DRACO_DEPLOYMENT_NAME" --query properties.outputs.azureSearchIndexerConfiguration.value > azure-search-indexer-config.json

SEARCH_SERVICE_NAME=$(az deployment group show --resource-group "$DRACO_PLATFORM_RG_NAME" --name "$DRACO_DEPLOYMENT_NAME" --query properties.outputs.azureSearchServiceName.value --output tsv)
COSMOS_ADMIN_KEY=$(az search admin-key show --resource-group "$DRACO_PLATFORM_RG_NAME" --service-name "$SEARCH_SERVICE_NAME" --query primaryKey --output tsv)

# Configure Azure Search...

echo -e "\nConfiguring search index data source...\n"

curl -X POST https://$SEARCH_SERVICE_NAME.search.windows.net/datasources\?api-version=2019-05-06 -d @azure-search-datasource-config.json --header "Content-Type: application/json" --header "api-key: $COSMOS_ADMIN_KEY"

echo -e "\n\nConfiguring search index...\n"

curl -X POST https://$SEARCH_SERVICE_NAME.search.windows.net/indexes\?api-version=2019-05-06 -d @azure-search-index-config.json --header "Content-Type: application/json" --header "api-key: $COSMOS_ADMIN_KEY"

echo -e "\n\nConfiguring search indexer...\n"

curl -X POST https://$SEARCH_SERVICE_NAME.search.windows.net/indexers\?api-version=2019-05-06 -d @azure-search-indexer-config.json --header "Content-Type: application/json" --header "api-key: $COSMOS_ADMIN_KEY"

rm "./azure-search-datasource-config.json"
rm "./azure-search-index-config.json"
rm "./azure-search-indexer-config.json"

# Get Draco service configuration from ARM deployment output...
echo -e "\nGetting Draco service configuration from deployment [$DRACO_DEPLOYMENT_NAME]..."

az deployment group show --resource-group "$DRACO_PLATFORM_RG_NAME" --name "$DRACO_DEPLOYMENT_NAME" --query properties.outputs.catalogApiConfiguration.value > appsettings-catalogapi.json
az deployment group show --resource-group "$DRACO_PLATFORM_RG_NAME" --name "$DRACO_DEPLOYMENT_NAME" --query properties.outputs.extensionMgmtApiConfiguration.value > appsettings-extensionmgmtapi.json
az deployment group show --resource-group "$DRACO_PLATFORM_RG_NAME" --name "$DRACO_DEPLOYMENT_NAME" --query properties.outputs.executionConsoleConfiguration.value > appsettings-execconsole.json
az deployment group show --resource-group "$DRACO_PLATFORM_RG_NAME" --name "$DRACO_DEPLOYMENT_NAME" --query properties.outputs.executionApiConfiguration.value > appsettings-execapi.json

# Upload Draco service configuration to blob storage...
echo -e "\nSaving Draco service configuration to platform storage account..."

DRACO_PLATFORM_STOR_CONN_STR=$(az deployment group show --resource-group "$DRACO_PLATFORM_RG_NAME" --name "$DRACO_DEPLOYMENT_NAME" --query properties.outputs.executionApiConfiguration.value.platforms.azure.objectStorage.blobStorage.storageAccount.connectionString --output tsv)

echo -e "\nUploading Draco catalog API configuration [appsettings-catalogapi.json] to blob storage...\n"
az storage blob upload --file "./appsettings-catalogapi.json" --connection-string "$DRACO_PLATFORM_STOR_CONN_STR" --container-name "configuration" --name "appsettings-catalogapi.json"

echo -e "\nUploading Draco extension management API configuration [appsettings-extensionmgmtapi.json] to blob storage...\n"
az storage blob upload --file "./appsettings-extensionmgmtapi.json" --connection-string "$DRACO_PLATFORM_STOR_CONN_STR" --container-name "configuration" --name "appsettings-extensionmgmtapi.json"

echo -e "\nUploading Draco execution agent configuration [appsettings-execconsole.json] to blob storage...\n"
az storage blob upload --file "./appsettings-execconsole.json" --connection-string "$DRACO_PLATFORM_STOR_CONN_STR" --container-name "configuration" --name "appsettings-execconsole.json"

echo -e "\nUploading Draco execution API configuration [appsettings-execapi.json] to blob storage...\n"
az storage blob upload --file "./appsettings-execapi.json" --connection-string "$DRACO_PLATFORM_STOR_CONN_STR" --container-name "configuration" --name "appsettings-execapi.json"

rm "./appsettings-catalogapi.json"
rm "./appsettings-extensionmgmtapi.json"
rm "./appsettings-execconsole.json"
rm "./appsettings-execapi.json"

# Connect to our AKS cluster...
AKS_CLUSTER_NAME=$(az resource list --resource-group "$DRACO_PLATFORM_RG_NAME" --resource-type "Microsoft.ContainerService/managedClusters" --query '[0].name' --output tsv)

echo -e "\n\nConnecting to AKS cluster [$AKS_CLUSTER_NAME]...\n"

az aks get-credentials --resource-group "$DRACO_PLATFORM_RG_NAME" --name "$AKS_CLUSTER_NAME"

# Build containers...
echo -e "\nBuilding Draco catalog API Docker container...\n"
docker build "../../../" --file "../../../api/Catalog.Api/Dockerfile" --tag "$ACR_NAME.azurecr.io/xhub-catalogapi:latest"

echo -e "\nBuilding Draco execution API Docker container...\n"
docker build "../../../" --file "../../../api/Execution.Api/Dockerfile" --tag "$ACR_NAME.azurecr.io/xhub-executionapi:latest"

echo -e "\nBuilding Draco extension management API Docker container...\n"
docker build "../../../" --file "../../../api/ExtensionManagement.Api/Dockerfile" --tag "$ACR_NAME.azurecr.io/xhub-extensionmgmtapi:latest"

echo -e "\nBuilding Draco execution adapter API Docker container...\n"
docker build "../../../" --file "../../../api/ExecutionAdapter.Api/Dockerfile" --tag "$ACR_NAME.azurecr.io/xhub-executionadapterapi:latest"

echo -e "\nBuilding Draco extension service API Docker container...\n"
docker build "../../../" --file "../../../api/ExtensionService.Api/Dockerfile" --tag "$ACR_NAME.azurecr.io/xhub-extensionserviceapi:latest"

echo -e "\nBuilding Draco object storage provider API Docker container...\n"
docker build "../../../" --file "../../../api/ObjectStorageProvider.Api/Dockerfile" --tag "$ACR_NAME.azurecr.io/xhub-objectproviderapi:latest"

echo -e "\nBuilding Draco execution adapter API Docker container...\n"
docker build "../../../" --file "../../../core/Agent/ExecutionAdapter.ConsoleHost/Dockerfile" --tag "$ACR_NAME.azurecr.io/xhub-executionconsole:latest"

# Push containers...
az acr login --name $ACR_NAME

echo -e "\nPushing Draco catalog API Docker container to [$ACR_NAME.azurecr.io]...\n"
docker push "$ACR_NAME.azurecr.io/xhub-catalogapi"

echo -e "\nPushing Draco execution API Docker container to [$ACR_NAME.azurecr.io]...\n"
docker push "$ACR_NAME.azurecr.io/xhub-executionapi"

echo -e "\nPushing Draco extension management API Docker container to [$ACR_NAME.azurecr.io]...\n"
docker push "$ACR_NAME.azurecr.io/xhub-extensionmgmtapi"

echo -e "\nPushing Draco execution agent Docker container to [$ACR_NAME.azurecr.io]...\n"
docker push "$ACR_NAME.azurecr.io/xhub-executionconsole"

echo -e "\nPushing Draco execution adapter API Docker container to [$ACR_NAME.azurecr.io]...\n"
docker push "$ACR_NAME.azurecr.io/xhub-executionadapterapi"

echo -e "\nPushing Draco extension service API Docker container to [$ACR_NAME.azurecr.io]...\n"
docker push "$ACR_NAME.azurecr.io/xhub-extensionserviceapi"

echo -e "\nPushing Draco object storage provider API Docker container to [$ACR_NAME.azurecr.io]...\n"
docker push "$ACR_NAME.azurecr.io/xhub-objectproviderapi"

echo -e "\nRemoving all local Draco containers...\n"

docker rmi "$ACR_NAME.azurecr.io/xhub-catalogapi" >/dev/null
docker rmi "$ACR_NAME.azurecr.io/xhub-executionapi" >/dev/null
docker rmi "$ACR_NAME.azurecr.io/xhub-extensionmgmtapi" >/dev/null
docker rmi "$ACR_NAME.azurecr.io/xhub-executionconsole" >/dev/null
docker rmi "$ACR_NAME.azurecr.io/xhub-executionadapterapi" >/dev/null
docker rmi "$ACR_NAME.azurecr.io/xhub-extensionserviceapi" >/dev/null
docker rmi "$ACR_NAME.azurecr.io/xhub-objectproviderapi" >/dev/null

echo -e "Deploying Draco Helm chart to AKS cluster [$AKS_CLUSTER_NAME]...\n"

helm install initial "../../../helm/extension-hubs" --set configuration.storageConnectionString="$DRACO_PLATFORM_STOR_CONN_STR" --set images.repository="$ACR_NAME.azurecr.io" --set dnsPrefix="$DEPLOYMENT_PREFIX"

echo -e "\nAll done!\n" 

echo -e "Run [kubectl get pods --watch] to monitor the deployment of Draco pods."
echo -e "Run [kubectl get services --watch] to monitor the deployment of Draco services and load balancers.\n"

echo -e "Once all of the service have been fully deployed, these will be your Draco API endpoints...\n"

echo "Draco Catalog API:               [http://$DEPLOYMENT_PREFIX-catalogapi.$LOCATION.cloudapp.azure.com]"
echo "Draco Execution API:             [http://$DEPLOYMENT_PREFIX-executionapi.$LOCATION.cloudapp.azure.com]"
echo "Draco Extension Management API:  [http://$DEPLOYMENT_PREFIX-extensionmgmtapi.$LOCATION.cloudapp.azure.com]"