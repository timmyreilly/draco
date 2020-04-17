# Register an Extension with the Draco platform

The following instructions are how to register an extension on the Draco platform.  These instructions will walk through building the extension, pushing it to ACR, and registering it with the draco platform.

> The following instructions have been tested using Ubuntu 18.04, Mac OS 10.15 and Windows 10.

## Pre-requisites

* Azure Subscription
* [Kubernetes CLI](https://kubernetes.io/docs/tasks/tools/install-kubectl/)
* [Docker](https://www.docker.com/products/docker-desktop)
* [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
* [Azure Kubernetes CLI / kubectl](https://docs.microsoft.com/en-us/cli/azure/aks?view=azure-cli-latest#az-aks-install-cli)
* [Draco Environment Pre-Setup](https://github.com/microsoft/draco/blob/master/doc/setup/README.md)

## Specific to MacOS

* [Home Brew](https://brew.sh/) to install the following tools:
* azure-cli (alternative install method to direct download)
* docker  (You still need Docker Desktop install from above)
* kubernetes-cli
* helm

## Login to Azure Subscription

Use the following commands to authenticate to Azure and set the Azure subscription you deployed Draco in.

```bash
az login

# If you have multiple subscriptions, set the subscription to use for Draco.
az account list 
az account set --subscription "Name of Subscription"
```

## Initialize Variables

> NOTE: These values must match the values used at initial setup

```bash
DRACO_COMMON_RG_NAME="draco-common-rg"
ACR_NAME=$(az acr list --resource-group $DRACO_COMMON_RG_NAME --query "[].name" --output tsv)
```

## Build sample echo container image

This step uses the `Dockerfile` for the extension to build sample echo container image and tag it for storage in ACR.

> NOTE: Execute from the root of the git enlistment

```bash
cd src/extensions/samples/csharp/netcore-simple/echo
docker build . --file Dockerfile --tag $ACR_NAME".azurecr.io/sample-echo:latest"
```

## Push echo container image to Azure Container Registry

```bash
az acr login --name $ACR_NAME
docker push $ACR_NAME".azurecr.io/sample-echo"
```

> NOTE: This will push to the Azure Container Registry (ACR). You can verify this in the portal via the ACR resource.

## Deploy to AKS

Using yaml file in the sample echo extension folder, deploy the echo extension into AKS.

> NOTE: Kubectl will not do variable expansion from with a yaml file.  You MUST edit the yaml file (at or around line 27) below and replace the {{ACR_NAME}} string with your actual ACR_NAME. Also, make sure there are no trailing spaces on the image name.

```bash
kubectl apply -f sample-echo.yaml
```

## Validate Extension service is running in AKS

To validate you have the extension service running in AKS, verify there is a pod running for the echo extension.

```bash
kubectl get pods
```

## Register the echo extension container with the Draco platform

Create a new extension registration for the echo service using the following rest calls.

* Replace {{extmgmt_url}} with the extension management URL from the the setup scripts.  This will be `http://draco-***-apim.azure-api.net/extensions`
* Replace ***ExtensionName*** with the name of your extension (sample-echo for this sample)

```json
API Request
HTTP Verb and URL:  
    Set Verb to POST
    Set URL to {{extmgmt_url}}/extensions

Request Headers:
    Content-Type: application/json

Request Payload (json)
Use this format:
{
    "name": "***ExtensionName***",
    "category": "Sample",
    "subcategory": "Test Extensions",
    "description": "Sample echo request for registration of an extension",
    "publisherName": "Microsoft",
    "copyrightNotice": "Copyright (c) Microsoft Corporation",
    "additionalUrls": {
    "publisherUrl": "https://microsoft.com",
    "dracoUrl": "https://github.com/microsoft/draco"
    },
    "isActive": true,
    "tags": [
    "echo",
    "sample"
    ]
}
```

The response from the service will include a new extension id that will be used below to finish the registration of a new service.

```json
API Response
{
    "links": null,
    "model": {
        "tags": [
            "echo",
            "sample"
        ],
        "id": "{{extension_id}}",
        "name": "ExtensionName",
        "category": "Sample",
        "subcategory": "Test Extensions",
        "coverImageUrl": null,
        "logoUrl": null,
        "description": "Sample echo extension for registration of an extension",
        "publisherName": "Microsoft",
        "copyrightNotice": "Copyright (c) Microsoft Corporation",
        "isActive": true,
        "additionalInformationUrls": {},
        "features": {}
    }
}
```

> NOTE:  Copy {{extension_id}} returned in the model section, you will need this in following steps.

## Create the sample echo extension version

Register a new extension version using the following steps:

* Replace {{extmgmt_url}} with the extension management URL from the the setup scripts (sames as above).
* Replace {{extension_id}} with the value from the previous step.
* Replace **ExtensionName** with the name of the extension.
* "version" should also be set to the version of the extension [x.xx format].

> Note:  Each extension can have multiple versions in the platform at the same time.

```json
API Request
HTTP Verb and URL:  
    Set Verb to POST
    Set URL to {{extmgmt_url}}/extensions/{{extension_id}}/versions

Request Headers:
    Content-Type: application/json

Request Payload
    Use this format (JSON):
    {
        "requestTypeName": "***ExtensionName***/requests/v1",
        "responseTypename": "***ExtensionName***/response/v1",
        "version": "***0.1***",
        "isLongRunning": false,
        "isActive": true,
        "supportsValidation": false,
        "executionExpirationPeriod": "00:10:00"
    }
```

The platform service will return a new ExtensionVersionId used to uniquely identify this specific extension and version as a paired entity.

```json
API response:
{
        "links": null,
        "model": {
            "id": "{{extension_version_id}}",
            "extensionId": "ExtensionId",
            "releaseNotes": null,
            "requestTypeName": "ExtensionName/requests/v1",
            "responseTypeName": "ExtensionName/response/v1",
            "requestTypeUrl": null,
            "responseTypeUrl": null,
            "version": "0.1",
            "isLongRunning": false,
            "isActive": true,
            "supportsValidation": false,
            "executionExpirationPeriod": null
        }
}
```

* Copy ***ExtensionVersionId*** from the *model* section

## Create echo extension execution profile

Each extension also needs an execution profile to be set for each version of an extension.  For the echo extension, this will set a default execution model for the extention to be synchronous.

**TODO** Replace with link to other execution models

* Replace {{extmgmt_url}} with the extension management URL from the the setup scripts (sames as above).
* Replace {{extension_id}}
* Replace {{extension_version_id}}
* Replace ***ExtensionIP*** as follows:
  - Retrieve the `EXTERNAL-IP` value of the Echo extension using `kubectl get svc'.

```json
API Request
HTTP Verb and URL:
    Set Verb to POST
    Set URL to {{extmgmt_url}}/extensions/{{extension_id}}/versions/{{extension_version_id}}/profiles

Request Headers:
    Content-Type: application/json

Request Payload (if applicable)
Use this format:
    {
        "name": "default",
        "executionModel": "http-json/sync/v1",
        "objectProvider": "az-blob/v1",
        "isActive": true,
        "executionMode": "Gateway",
        "supportedPriorities": [
        "normal"
        ],
        "extensionSettings":
        {
            "executionUrl": "http://***ExtensionIP***/echo"
        }
    }
```

The platform service will return success for registration.

```json
API response:
{
    "links": null,
    "model": {
        "name": "default",
        "extensionId": "{{extension_id}}",  # Matches value passed in
        "extensionVersionId": "{{extension_version_id}}", # Matches value passed in
        "description": null,
        "executionModel": "http-json/sync/v1",
        "objectProvider": "az-blob/v1",
        "executionMode": "Gateway",
        "directExecutionTokenDuration": null,
        "isActive": true,
        "supportedPriorities": [
            "Normal"
        ],
        "clientConfiguration": {},
        "extensionSettings": {
            "executionUrl": "***ExtensionExecutionUrl***"  # Should match value passed in
        }
    }
}
```

## Sample echo extension is now registered and ready to be executed

See the [how to execute a Draco extension](Execute-Extension.md) for this newly registered echo extension.
