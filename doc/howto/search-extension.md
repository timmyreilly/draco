# Search For an Extension with the Draco platform

The following instructions are how to search for an extension on the Draco platform.  These instructions will walk through making a catalog API call to search the draco platform for extensions.

> The following instructions have been tested using Ubuntu 18.04, Mac OS 10.15 and Windows 10.

## Pre-requisites

* [Draco Environment Pre-Setup](https://github.com/microsoft/draco/blob/master/doc/setup/README.md)

## Search the Draco platform Catalog API for an extension

* Replace {{catalog_url}} with the catalog URL from the setup scripts.  This will be `http://draco-***-apim.azure-api.net/catalog`.
* Replace echo (sample extension name) with the search term or Extension Name of your Extension

```json
API Request
HTTP Verb and URL:  
    Set Verb to GET
    Set URL to http://{{catalog_url}}/catalog/search/?q=echo

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

* Replace {{catalog_url}} with the catalog URL from the setup scripts.  This will be `http://draco-***-apim.azure-api.net/catalog`.
* Replace Sample with the category term or Extension category of your Extension

```json
API Request
HTTP Verb and URL:  
    Set Verb to GET
    Set URL to http://{{catalog_url}}/catalog/search/Sample

```
```json
API Response

    Same as Extension search above
```

## Search the Draco platform Catalog API for a subcategory

* Replace {{catalog_url}} with the catalog URL from the setup scripts.  This will be `http://draco-***-apim.azure-api.net/catalog`.
* Replace .Net Core Samples with the subcategory term or Extension subcategory of your Extension

```json
API Request
HTTP Verb and URL:  
    Set Verb to GET
    Set URL to http://{{catalog_url}}/catalog/search/Sample/.NET Core Samples
```

```json
API Response

    Same as Extension search above
```
