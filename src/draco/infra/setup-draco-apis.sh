#!/bin/bash -e

# This script configures the Draco API's in API Management

usage() { echo "Usage: $0 <-c common-rg-name>"; }

while getopts "c:" opt; do
    case $opt in
        c)
            DRACO_COMMON_RG_NAME=$OPTARG # [Required] 
        ;;
        \?)
            usage
            exit 1
        ;;
    esac
done

[[ -z $DRACO_COMMON_RG_NAME ]] && { usage; exit 1; }

# Get the current subscription ID
SUBSCRIPTION_ID=$(az account show --query id --output tsv)

# Get the internal load balancer IP addres for each Draco service
EXTMGMT_API_IP_ADDRESS=$(kubectl get svc -o=jsonpath='{.items[?(@.metadata.name=="initial-extensionmgmtapi")].status.loadBalancer.ingress[0].ip}')
CATALOG_API_IP_ADDRESS=$(kubectl get svc -o=jsonpath='{.items[?(@.metadata.name=="initial-catalogapi")].status.loadBalancer.ingress[0].ip}')
EXECUTION_API_IP_ADDRESS=$(kubectl get svc -o=jsonpath='{.items[?(@.metadata.name=="initial-executionapi")].status.loadBalancer.ingress[0].ip}')

[[ -z "$EXTMGMT_API_IP_ADDRESS" || -z "$CATALOG_API_IP_ADDRESS" || -z "$EXECUTION_API_IP_ADDRESS" ]] && {
    echo "ERROR:"
    echo "  One or more of the Draci API internal addresses are missing."
    echo "  Please run this script again after the internal addresses are asssigned."
    exit 1
}

# Get an access token to authenticate to the Azure Management REST API's
AZURE_MGMT_ACCESS_TOKEN=$(az account get-access-token --query accessToken --output tsv)

[[ -z "$AZURE_MGMT_ACCESS_TOKEN" ]] && {
    echo "ERROR:"
    echo "  Please authenticate to Azure and then rerun this script."
    echo "  To authenticate run 'az login'."
    exit 1
}

AZURE_MGMT_URL="https://management.azure.com"
APIM_RESOURCE_NAME=$(az apim list --resource-group $DRACO_COMMON_RG_NAME --query "[].name | [0]" --output tsv)

echo "Setting up Draco Extension Management API in API Management."
# Replace the backend IP address of the service API's in the JSON payload to create the API's.
sed "s/internal-load-balancer-ip-address/$EXTMGMT_API_IP_ADDRESS/g" extension-mgmt-api-swagger.json > extension-mgmt-api-swagger-substitution.json
# Create the extension management API
curl -X PUT $AZURE_MGMT_URL/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$DRACO_COMMON_RG_NAME/providers/Microsoft.ApiManagement/service/$APIM_RESOURCE_NAME/apis/extensions?api-version=2019-12-01 -d @extension-mgmt-api-swagger-substitution.json --header "Authorization: Bearer $AZURE_MGMT_ACCESS_TOKEN" --header "Content-Type: application/json"
rm ./extension-mgmt-api-swagger-substitution.json

echo "Setting up Draco Catalog API in API Management."
# Replace the backend IP address of the service API's in the JSON payload to create the API's.
sed "s/internal-load-balancer-ip-address/$CATALOG_API_IP_ADDRESS/g" catalog-api-swagger.json > catalog-api-swagger-substitution.json
# Create the catalog API
curl -X PUT $AZURE_MGMT_URL/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$DRACO_COMMON_RG_NAME/providers/Microsoft.ApiManagement/service/$APIM_RESOURCE_NAME/apis/catalog?api-version=2019-12-01 -d @catalog-api-swagger-substitution.json --header "Authorization: Bearer $AZURE_MGMT_ACCESS_TOKEN" --header "Content-Type: application/json"
rm ./catalog-api-swagger-substitution.json

echo "Setting up Draco Execution API in API Management."
# Replace the backend IP address of the service API's in the JSON payload to create the API's.
sed "s/internal-load-balancer-ip-address/$EXECUTION_API_IP_ADDRESS/g" execution-api-swagger.json > execution-api-swagger-substitution.json
# Create the catalog API
curl -X PUT $AZURE_MGMT_URL/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$DRACO_COMMON_RG_NAME/providers/Microsoft.ApiManagement/service/$APIM_RESOURCE_NAME/apis/execution?api-version=2019-12-01 -d @execution-api-swagger-substitution.json --header "Authorization: Bearer $AZURE_MGMT_ACCESS_TOKEN" --header "Content-Type: application/json"
rm ./execution-api-swagger-substitution.json

echo ""
echo "The Draco API configuration is complete!"
echo ""
echo "Access each of the API's using the following URL's:"
echo ""
echo "Execution API:      http://$APIM_RESOURCE_NAME.azure-api.net/execution"
echo "Extension Mgmt API: http://$APIM_RESOURCE_NAME.azure-api.net/extensions"
echo "Catalog API:        http://$APIM_RESOURCE_NAME.azure-api.net/catalog"
echo ""
