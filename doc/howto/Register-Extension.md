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

> NOTE: This may launch a browser to validate your credentials.

```bash
az login
```

## Ensure correct Azure subscription/ Connected to the proper Kube instance is in use

If you have more than one Azure subscription make sure you choose the correct subscription before running any of these commands.

```bash
az account list  
az account set --subscription "Name of Subscription"
```

## Initialize Variables

> NOTE: These values must match the values used at initial setup

```bash
DRACO_COMMON_RG_NAME="draco-common-rg"
DRACO_PLATFORM_RG_NAME="draco-platform-rg"
DRACO_REGION="eastus2"
DRACO_AKS_CLUSTER=$(az aks list --resource-group $DRACO_PLATFORM_RG_NAME --query "[].name" --output tsv)
ACR_NAME=$(az deployment group show --resource-group $DRACO_COMMON_RG_NAME --name common-deploy --query properties.outputs.acrName.value --output tsv)
```

## Build sample echo container image

This step uses the `Dockerfile` for the extension to build sample echo container image and tag it for storage in ACR.

> NOTE: Execute from the root of the git enlistment

```bash
cd src/extensions/samples/csharp/netcore-simple/echo
docker build . --file Dockerfile --tag $ACR_NAME.azurecr.io/echo:latest
```

## Push echo container image to Azure Container Registry

```bash
az acr login --name $ACR_NAME
docker push $ACR_NAME".azurecr.io/echo"
```

> NOTE: This will push to the Azure Container Registry (ACR). You can verify this in the portal via the ACR resource.

## Deploy to AKS

Using yaml file in the sample echo extension folder, deploy the echo extension into AKS.

```bash
kubectl apply -f extension-echo.yaml
```

## Validate Extension service is running in AKS

To validate you have the extension service running in AKS.  You should see the extension listed with Name, type, Cluster IP, External-IP, Ports, and age.

```bash
kubectl get services
```

## Get the DNS/IP address for the Extension Management API

Get the extension management api External IP address from the Kubectl get services call above.

```bash
NAME                       TYPE           CLUSTER-IP     EXTERNAL-IP     PORT(S)                      AGE
extensionecho1             LoadBalancer   10.0.117.50    X.X.X.X   80:30424/TCP,443:30466/TCP   21h
initial-catalogapi         LoadBalancer   10.0.119.97    X.X.X.X   80:32542/TCP,443:32275/TCP   22h
initial-executionapi       LoadBalancer   10.0.246.229   X.X.X.X   80:30844/TCP,443:31053/TCP   22h
initial-extensionmgmtapi   LoadBalancer   10.0.150.241   X.X.X.X   80:32743/TCP,443:32610/TCP   22h
kubernetes                 ClusterIP      10.0.0.1       <none>    443/TCP                      22h
```

In the above example the **initial-extensionmgmtapi** EXTERNAL-IP is the value we need below.

## Register the echo extension container with the Draco platform

Create a new extension registration for the echo service using the following rest calls.

* Replace address with your DNS or IP address
* Replace ***ExtensionName*** with the name of your extension (echo for this sample)

```json
API Request
HTTP Verb and URL:  
    Set Verb to POST
    Set URL to http://address/extensions

Request Headers:
    Content-Type: application/json

Request Payload (json)
Use this format:
{
    "name": "***ExtensionName***",
    "category": "Demo",
    "subcategory": "Test Extensions",
    "description": "Test request for registration of an extension",
    "publisherName": "Microsoft",
    "copyrightNotice": "Copyright (c) Microsoft Corporation",
    "additionalUrls": {
    "publisherUrl": "https://microsoft.com",
    "dracoUrl": "https://github.com/microsoft/draco"
    },
    "isActive": true,
    "tags": [
    "test",
    "echo",
    "demo"
    ]
}
```

The response from the service will include a new extension id that will be used below to finish the registration of a new service.

```json
API Response
{
    "links": {
        "getExtension": "http://address/extensions/ExtensionID",
        "getExtensionVersions": "http://address/extensions/ExtensionID",
        "deleteExtension": "http://address/extensions/ExtensionID",
        "postNewExtension": "http://address/extensions",
        "postNewExtensionVersion": "http://address/extensions/ExtensionID/versions"
    },
    "model": {
                    "tags": [
                        "test",
                        "echo",
                        "demo"
                    ],
                    "id": "***ExtensionID***",
                    "name": "ExtensionName",
                    "category": "Demo",
                    "subcategory": "Test Extensions",
                    "coverImageUrl": null,
                    "logoUrl": null,
                    "description": "Test extension for registration of an extension",
                    "publisherName": "Microsoft",
                    "copyrightNotice": "Copyright (c) Microsoft Corporation",
                    "isActive": true,
                    "additionalInformationUrls": {},
                    "features": {}
                }
}
```

> NOTE:  Copy **ExtensionID** returned in the model section, you will need this in following steps.

## Create the sample echo extension version

Register a new extension version using the following steps:

* Replace address with your DNS or IP address.
* Replace **ExtensionId** with the value from the previous step.
* Replace **ExtensionName** with the name of the extension.
* "version" should also be set to the version of the extension [x.xx format].

> Note:  Each extension can have multiple versions in the platform at the same time.

```json
API Request
HTTP Verb and URL:  
    Set Verb to POST
    Set URL to address/extensions/***ExtensionId***/versions

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
        "links": {
            "getExtension": "http://address/extensions/ExtensionId",
            "getExtensionVersion": "http://address/extensions/ExtensionId/versions/ExtensionVersionId",
            "getAllExtensionVersions": "http://address/extensions/ExtensionId/versions",
            "getAllExecutionProfiles": "http://address/extensions/ExtensionId/versions/ExtensionVersionId/profiles",
            "getAllInputObjects": "http://address/extensions/ExtensionId/versions/ExtensionVersionId/objects/input",
            "getAllOutputObjects": "http://address/extensions/ExtensionId/versions/ExtensionVersionId/objects/output",
            "getAllServices": "http://address/extensions/ExtensionId/versions/ExtensionVersionId/services",
            "deleteExtension": "http://address/extensions/ExtensionId",
            "deleteExtensionVersion": "http://address/extensions/ExtensionId/versions/ExtensionVersionId",
            "postNewExtension": "http://address/extensions",
            "postNewExtensionVersion": "http://address/extensions/ExtensionId/versions",
            "postNewExecutionProfile": "http://address/extensions/ExtensionId/versions/ExtensionVersionId/profiles",
            "postNewInputObject": "http://address/extensions/ExtensionId/versions/ExtensionVersionId/objects/input",
            "postNewOutputObject": "http://address/extensions/ExtensionId/versions/ExtensionVersionId/objects/output",
            "postNewService": "http://address/extensions/ExtensionId/versions/ExtensionVersionId/services"
        },
        "model": {
            "id": "***ExtensionVersionId***",
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
* The ExtensionId and ExtensionName should be unchanged

## Create echo extension execution profile

Each extension also need an execution profile to be set for the version.  For the echo extension, this will set a default execution model for the extention to be synchronous.

**TODO** Replace with link to other execution models

* Replace address with your DNS or IP address
* Replace ***ExtensionId***
* Replace ***ExtensionVersionId***
* Replace ***ExtensionName*** extension name

```json
API Request
HTTP Verb and URL:
    Set Verb to POST
    Set URL to DNSname/extensions/***ExtensionId***/versions/***ExtensionVersionId***/profiles

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
        "extensionSettings": {
        "executionUrl": "address/***ExtensionName***"
        }
    }
```

The platform service will return the **executionUrl** from the params above.

```json
API response:
{
    "links": {
        "getExtension": "http://address/extensions/ExtensionId",
        "getExtensionVersion": "http://address/extensions/ExtensionId/versions/ExtensionVersionId",
        "getAllExtensionVersions": "http://address/extensions/ExtensionId/versions",
        "getExecutionProfile": "http://address/extensions/ExtensionId/versions/ExtensionVersionId/profiles/default",
        "getAllExecutionProfiles": "http://address/extensions/ExtensionId/versions/ExtensionVersionId/profiles",
        "deleteExtension": "http://address/extensions/ExtensionId",
        "deleteExtensionVersion": "http://address/extensions/ExtensionId/versions/ExtensionVersionId",
        "deleteExecutionProfile": "http://address/extensions/ExtensionId/versions/ExtensionVersionId/profiles/default",
        "postNewExtension": "http://address/extensions",
        "postNewExtensionVersion": "http://address/extensions/ExtensionId/versions",
        "postNewExecutionProfile": "http://address/extensions/ExtensionId/versions/ExtensionVersionId/profiles"
    },
    "model": {
        "name": "default",
        "extensionId": "ExtensionId",
        "extensionVersionId": "ExtensionVersionId",
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
            "executionUrl": "***address/ExtensionName***"
        }
    }
}
```

## Sample echo extension is now registered and ready to be executed

See the [how to execute a Draco extension](Execute-Extension.md) for this newly registered echo extension.
