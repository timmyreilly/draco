# Search For an Extension with the Draco platform

The following instructions are how to search for an extension on the Draco platform.  These instructions will walk through making a catalog API call to search the draco platform for extensions.

> The following instructions have been tested using Ubuntu 18.04, Mac OS 10.15 and Windows 10.

## Pre-requisites

* [Azure Kubernetes CLI / kubectl](https://docs.microsoft.com/en-us/cli/azure/aks?view=azure-cli-latest#az-aks-install-cli)
* [Draco Environment Pre-Setup](https://github.com/microsoft/draco/blob/master/doc/setup/README.md)

## Specific to MacOS

* [Home Brew](https://brew.sh/) to install the following tools:
* kubernetes-cli

## Validate Extension service is running in AKS

To validate you have the extension service running in AKS.  You should see the extension listed with Name, type, Cluster IP, External-IP, Ports, and age. The kubectl get services will display the services running and the information needed.

```bash
kubectl get services
```

## Get the DNS/IP address for the Catalog API

Get the Catalog API External IP address from the Kubectl get services call above.

```bash
NAME                       TYPE           CLUSTER-IP     EXTERNAL-IP     PORT(S)                      AGE
extensionecho1             LoadBalancer   10.0.117.50    X.X.X.X   80:30424/TCP,443:30466/TCP   21h
initial-catalogapi         LoadBalancer   10.0.119.97    X.X.X.X   80:32542/TCP,443:32275/TCP   22h
initial-executionapi       LoadBalancer   10.0.246.229   X.X.X.X   80:30844/TCP,443:31053/TCP   22h
initial-extensionmgmtapi   LoadBalancer   10.0.150.241   X.X.X.X   80:32743/TCP,443:32610/TCP   22h
kubernetes                 ClusterIP      10.0.0.1       <none>    443/TCP                      22h
```

In the above example the **initial-catalogapi** EXTERNAL-IP is the value we need below.

## Search the Draco platform Catalog API for an extension

* Replace address with your DNS or IP address from the catalogAPI - External IP above.
* Replace echo (sample extension name) with the search term or Extension Name of your Extension

```json
API Request
HTTP Verb and URL:  
    Set Verb to GET
    Set URL to http://address/catalog/search/?q=echo

```

The response from the service will return a status 200 OK - and will return the information on the echo (or your own) extension.

```json
API Response
    {
        {
            "pageIndex": 0,
            "pageSize": 1,
            "totalPages": 1,
            "totalResults": 1,
            "results": [
                {
                    "id": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
                    "name": "Echo",
                    "publisherName": "Microsoft",
                    "description": "Echoes back the request data to the caller.",
                    "category": "Sample",
                    "subcategory": ".NET Core Samples",
                    "getExtensionDetailUrl": "http://XX.XXX.XXX.XXX/catalog/extensions/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX?searchId=XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX&actionId=XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
                    "tags": [
                        "echo",
                        "test",
                        "demo"
                    ]
                }
            ]
        }
    }
```

## Search the Draco platform Catalog API for a category

* Replace address with your DNS or IP address from the catalogAPI - External IP above.
* Replace Sample with the category term or Extension category of your Extension

```json
API Request
HTTP Verb and URL:  
    Set Verb to GET
    Set URL to http://address/catalog/search/Sample

```
```json
API Response

    Same as Extension search above
```

## Search the Draco platform Catalog API for a subcategory

* Replace address with your DNS or IP address from the catalogAPI - External IP above.
* Replace .Net Core Samples with the subcategory term or Extension subcategory of your Extension

```json
API Request
HTTP Verb and URL:  
    Set Verb to GET
    Set URL to http://address/catalog/search/Sample/.NET Core Samples
```

```json
API Response

    Same as Extension search above
```
