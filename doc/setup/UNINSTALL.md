# Uninstall the Draco platform components

> The following instructions only work if you performed the initial setup using the [Setup Readme](README.md).

## Initialize variables

These values must match the ones used during intial setup.

```bash
DRACO_COMMON_RG_NAME="draco-common-rg"
DRACO_EXTHUB_RG_NAME="draco-exthub-rg"
DRACO_REGION="eastus2"
DRACO_SPN_K8S="http://draco-k8s-***UNIQUE*VALUE***"  
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

## Remove your Draco environment

**Do not execute the below commands unless you want to completely remove all Draco components from your Azure subscription.**

```bash
az ad sp delete --id $DRACO_SPN_K8S
az group delete --name $DRACO_COMMON_RG_NAME --yes --no-wait
az group delete --name $DRACO_EXTHUB_RG_NAME --yes --no-wait
```
