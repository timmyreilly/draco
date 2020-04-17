#!/bin/bash -e

# This script sets up all the common Draco components in Azure.
# For more information, see /doc/setup/README.md.

usage() { echo "Usage: $0 <-l azure-location> <-n aks-sp-name> [-e apim-publisher-email] [-p apim-publisher-name ] [-r resource-group]"; }

DRACO_DEPLOYMENT_NAME="deploy-common-$(date -u +%m%d%Y-%H%M%S)"
DRACO_COMMON_RG_NAME="draco-common-rg"
DRACO_SPN_AKV_KEY_NAME="draco-akv-spn"
DRACO_COMMON_TEMPLATE_PATH="./ArmTemplate/common/common-deploy.json"
APIM_PUBLISHER_NAME="Project Draco"

while getopts "e:l:n:p:r:" opt; do
    case $opt in
        e)
            APIM_PUBLISHER_EMAIL=$OPTARG  # [Optional] -- If not provided, the script will use the email from the Azure account.
        ;;
        l)
            LOCATION=$OPTARG # [Required] -- The Azure location that common resources should be deployed to.
        ;;
        n)
            SPN_AKS=$OPTARG # [Required] -- The AKS AAD service principal name.
        ;;
        p)
            APIM_PUBLISHER_NAME=$OPTARG  # [Optional] -- If not provided, uses $APIM_PUBLISHER_NAME from above.
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
[[ -z $LOCATION || -z $SPN_AKS ]] && { usage; exit 1; }

# Trim leading/trailing whitespaces from params
LOCATION=$(echo $LOCATION | tr -d '[[:space:]]')
SPN_AKS=$(echo $SPN_AKS | tr -d '[[:space:]]')

# Get the Azure account email if one isn't provided.  Will use this for the APIM Publisher Email.
[[ -z $APIM_PUBLISHER_EMAIL ]] && { 
    APIM_PUBLISHER_EMAIL=$(az account show --query user.name --output tsv)
}

# Validate location.
LOCATION=$(az account list-locations --query "[?name=='$LOCATION'] | [0].name" --output tsv)
[[ -z $LOCATION ]] && {
    echo "Invalid location (-l) provided.  Use 'az account list-locations' to see valid locations."
    exit 1
}

# Create the common resource group if it doesn't already exist...
if [[ -z $(az group list --query "[?name=='$DRACO_COMMON_RG_NAME']" --output tsv) ]]; then
    echo -e "\nCreating Draco common resource group [$DRACO_COMMON_RG_NAME]...\n"
    az group create --location "$LOCATION" --name "$DRACO_COMMON_RG_NAME" --tags platform=draco
else
    echo -e "\nDraco common resource group [$DRACO_COMMON_RG_NAME] already exists."
fi

# Deploy the common resources and get the ACR ID...
echo -e "\nDeploying common Draco resources to [$DRACO_COMMON_RG_NAME].  This can take approximately 35 minutes...\n"
az deployment group create --verbose --name "$DRACO_DEPLOYMENT_NAME" --resource-group "$DRACO_COMMON_RG_NAME" --template-file $DRACO_COMMON_TEMPLATE_PATH  --parameters apimPublisherEmail="$APIM_PUBLISHER_EMAIL" apimPublisherName="$APIM_PUBLISHER_NAME" 

# Get the ACR ID and Key Vault name. We'll need both later...
echo "Retrieving outputs from template deployment..."
ACR_ID=$(az deployment group show --resource-group "$DRACO_COMMON_RG_NAME" --name "$DRACO_DEPLOYMENT_NAME" --query properties.outputs.acrResourceId.value --output tsv)
AKV_NAME=$(az deployment group show --resource-group "$DRACO_COMMON_RG_NAME" --name "$DRACO_DEPLOYMENT_NAME" --query properties.outputs.kvName.value --output tsv)

# Create a new service principal...
echo -e "\nCreating a new AAD service prinicipal [$SPN_AKS] and assigning it the appropriate permissions...\n"
SPN_PASSWORD=$(az ad sp create-for-rbac --name "$SPN_AKS" --scopes "$ACR_ID" --role acrpull --query password --output tsv)
SPN_APP_ID=$(az ad sp show --id "$SPN_AKS" --query appId --output tsv)
UPN=$(az account show --query user.name --output tsv)

# Save the service principal password as a secret to the AKV we just created...
echo -e "\nSaving service principal password to key vault [$AKV_NAME]...\n"
az keyvault set-policy --name "$AKV_NAME" --upn $"$UPN" --secret-permissions get list set
az keyvault secret set --vault-name "$AKV_NAME" --name "$DRACO_SPN_AKV_KEY_NAME" --value "$SPN_PASSWORD"

echo ""
echo "The Draco 'common' infrastructure setup is complete!"
echo ""
echo "Common Resource Group Name:     [$DRACO_COMMON_RG_NAME]"
echo "AKS Service Principal ID:       [$SPN_APP_ID]"
echo ""
echo "Next, run the following command to setup the Draco 'platform' infrastructure:"
echo ""
echo "./setup-platform.sh -a \"$SPN_APP_ID\" -c \"$DRACO_COMMON_RG_NAME\""
echo ""

