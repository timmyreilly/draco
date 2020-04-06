#!/bin/bash -e

# This script sets up all the common Draco components in Azure.
# For more information, see /doc/setup/README.md.

usage() { echo "Usage: $0 <-l azure-location> <-n aks-sp-name> <-s subscription-id> [-r resource-group]"; }

DRACO_DEPLOYMENT_NAME="deploy-common-$(date -u +%m%d%Y-%H%M%S)"
DRACO_COMMON_RG_NAME="draco-common-rg"
DRACO_SPN_AKV_KEY_NAME="draco-akv-spn";

while getopts "s:l:n:r:" opt; do
    case $opt in
        s)
            SUBSCRIPTION_ID=$OPTARG # [Required]
        ;;
        l)
            LOCATION=$OPTARG # [Required] -- The Azure location that common resources should be deployed to.
        ;;
        n)
            SPN_AKS=$OPTARG # [Required] -- The AKS AAD service principal name.
        ;;
        r)
            DRACO_COMMON_RG_NAME=$OPTARG # [Optional] -- The desired target resource group name. If not provided, uses "draco-common-rg".
        ;;
        \?)
            usage
            exit 1
        ;;
    esac
done

# Check to make sure we have all the arguments that we need...
[[ -z $LOCATION || -z $SPN_AKS || -z $SUBSCRIPTION_ID ]] && { usage; exit 1; }

# Make sure we're logged into Azure...
[[ -z $(az account list --refresh --output tsv) ]] && az login

# Switch to the right subscription...
az account set --subscription "$SUBSCRIPTION_ID"

# Create the common resource group if it doesn't already exist...
if [[ -z $(az group list --query "[?name=='$DRACO_COMMON_RG_NAME']" --output tsv) ]]; then
    echo -e "\nCreating Draco common resource group [$DRACO_COMMON_RG_NAME]...\n"
    az group create --location "$LOCATION" --name "$DRACO_COMMON_RG_NAME" --tags platform=draco
else
    echo -e "\nDraco common resource group [$DRACO_COMMON_RG_NAME] already exists."
fi

echo -e "\nDeploying common Draco resources to [$DRACO_COMMON_RG_NAME]...\n"

# Deploy the common resources and get the ACR ID...
az deployment group create --name "$DRACO_DEPLOYMENT_NAME" --resource-group "$DRACO_COMMON_RG_NAME" --template-file "./common-deploy.json"

# Get the ACR ID and name. We'll need both later...
ACR_ID=$(az deployment group show --resource-group "$DRACO_COMMON_RG_NAME" --name "$DRACO_DEPLOYMENT_NAME" --query properties.outputs.acrResourceId.value --output tsv)
ACR_NAME=$(az deployment group show --resource-group "$DRACO_COMMON_RG_NAME" --name "$DRACO_DEPLOYMENT_NAME" --query properties.outputs.acrName.value --output tsv)

# Get the AKV name. We'll need it later...
AKV_NAME=$(az deployment group show --resource-group "$DRACO_COMMON_RG_NAME" --name "$DRACO_DEPLOYMENT_NAME" --query properties.outputs.kvName.value --output tsv)

echo -e "\nCreating a new AAD service prinicipal [$SPN_AKS] and assigning it the appropriate permissions...\n"

# Create a new service principal...
SPN_PASSWORD=$(az ad sp create-for-rbac --name "$SPN_AKS" --scopes "$ACR_ID" --role acrpull --query password --output tsv)
SPN_APP_ID=$(az ad sp show --id "$SPN_AKS" --query appId --output tsv)
UPN=$(az account show --query user.name --output tsv)

echo -e "\nSaving service principal password to key vault [$AKV_NAME]...\n"

# Save the service principal password as a secret to the AKV we just created...
az keyvault set-policy --name "$AKV_NAME" --upn $"$UPN" --secret-permissions get list set
az keyvault secret set --vault-name "$AKV_NAME" --name "$DRACO_SPN_AKV_KEY_NAME" --value "$SPN_PASSWORD"

echo -e "\n"

echo "Common Resource Group Name:     [$DRACO_COMMON_RG_NAME]"
echo "Deployment Name:                [$DRACO_DEPLOYMENT_NAME]"
echo "Container Registry ID:          [$ACR_ID]"
echo "Container Registry Name:        [$ACR_NAME]"
echo "Key Vault Name:                 [$AKV_NAME]"
echo "AKS Service Principal ID:       [$SPN_APP_ID]"
echo "AKS Service Principal Password: [$SPN_PASSWORD]"














