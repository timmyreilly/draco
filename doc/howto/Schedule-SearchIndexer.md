# Schedule Search Indexer to run on the Draco platform

The following instructions are how to schdule search indexer on the Draco platform.  These instructions will walk through making a search indexer API call to update the inital created indexer in setup on the draco platform for extensions.

The indexer needs to run on a schedule in order to update the index for new extensions. This allows extensions to surface in the catalog. If you need help searching for an extension, execution, or registering extensions, please see the other howto docs found her {{**TODO Add links to other howto docs**}}

> The following instructions have been tested using Ubuntu 18.04, Mac OS 10.15 and Windows 10.

## Pre-requisites

* [Azure Kubernetes CLI / kubectl](https://docs.microsoft.com/en-us/cli/azure/aks?view=azure-cli-latest#az-aks-install-cli)
* [Draco Environment Pre-Setup](https://github.com/microsoft/draco/blob/master/doc/setup/README.md)
* [Azure Portal access and initial naming from draco platform setup]({{**TODO Add Link here**}})

## Specific to MacOS

* [Home Brew](https://brew.sh/) to install the following tools:
* kubernetes-cli

## Needed Variables and Naming

When the draco platform is setup intially there are some names assigned and those will be needed here. This is a list needed to update the search indexer to run on a schedule.

Search Service Name
: This is defined on draco platform setup. Set as uniqueID defined with '-srch' on the end. Can also be found in the portal under search service
Index Name
: This is defined on draco platform setup. Set as uniqueID defined with '-index' on the end. Can also be found in the portal under search service overview tab, then indexes.
Indexer Name
: This is defined on draco platform setup. Set as uniqueID defined with '-indexer' on the end. Can also be found in the portal under search service overview tab, then indexers.
Data Source Name
: Set as 'extensions'
Search Service Admin/API Key
: Found in portal on Search Service then under keys. Portal names it admin key, the API call will use API-key

## Serch Service RestAPI call to schedule indexer to run on an interval

* Replace any variables in "***Var***" format with your own naming/variables.

```json
API Request
HTTP Verb and URL:  
    Set Verb to PUT
    Set URL to https://[search service name].search.windows.net/indexers/[indexer name]?api-version=[api-version]
    Content-Type: application/json  
    api-key: [admin key]    

Request Payload (json)
Use this format:

    {
        "dataSourceName": "extensions",
        "targetIndexName": "[uniqueID]-index",
        "schedule" : { "interval" : "PT10M"},  
        "parameters" : { "maxFailedItems" : 10, "maxFailedItemsPerBatch" : 5 },
        "configuration" : { "assumeOrderByHighWaterMarkColumn" : true }
    }

```

The response from the service will return a status 200 OK - and will return the information on the Search Service.

```json
API Response
    {
        "@odata.context": "https://[SearchServiceName]srch.search.windows.net/$metadata#indexers/$entity",
        "@odata.etag": "\"0x8DXXXXXXXXXXXXX\"",
        "name": "[uniqueID]-indexer",
        "description": null,
        "dataSourceName": "extensions",
        "skillsetName": null,
        "targetIndexName": "[uniqueID]-index",
        "disabled": null,
        "schedule": {
            "interval": "PT10M",
            "startTime": "0001-01-01T00:00:00Z"
        },
        "parameters": {
            "batchSize": null,
            "maxFailedItems": 10,
            "maxFailedItemsPerBatch": 5,
            "base64EncodeKeys": null,
            "configuration": {}
        },
        "fieldMappings": [],
        "outputFieldMappings": [],
        "cache": null
    }
```

### URI Parameters
<dl>    
<dt>service name</dt>
    <dd>Required. Set this to the unique, user-defined name of your search service.</dd>
<dt>indexer name</dt>
    <dd>Required. The request URI specifies the name of the indexer to run.</dd>
<dt>api-version</dt>
    <dd>Required. The current version is api-version=2019-05-06. See API versions in Azure Cognitive Search for a list of available versions.</dt>
</dl>


### Request Headers
<dl>
<dt>api-key</dt>
    <dd>Required. The api-key is used to authenticate the request to your Search service. It is a string value, unique to your service. Get requests about objects in your service must include an api-key field set to your admin key (as opposed to a query key).</dd>
</dl>
---
> NOTE:  Replace any variables in [RedHighlight] with your own naming/variables.
---
> NOTE:  Schedule params - Interval is set as a time interval in the format of PT Time then H/M for hours or minutes. Example: PT10M will run the indexer every 10 minutes. PT2H would run the indexer every 2 hours. This can be set up to 24 hours. 
---
## Serch Service RestAPI call to schedule indexer to run on demand

```json
API Request
HTTP Verb and URL:  
    Set Verb to POST
    Set URL to https://[search service name].search.windows.net/indexers/[indexer name]/run?api-version=[api-version]
    Content-Type: application/json  
    api-key: [admin key]    

Request Payload (json)
Body:

```

The response from the service will return a status 202 Accepted - The indexer will run on demand once accepted.

