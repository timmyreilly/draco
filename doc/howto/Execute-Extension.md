# Executing an extension from your application

The following instructions are how to execute a Draco platform extension. 

> The following instructions have been tested and verified using Ubuntu 18.04 

## Pre-requisites

* [Draco Environment Pre-Setup](https://github.com/microsoft/draco/blob/master/doc/setup/README.md)
* Review sample [echo extension](../../src/extensions/samples/csharp/netcore-simple/echo)
* Your extension is already [registered](Register-Extension.md) in the Draco platform.  As part of the extension registration you should have an _extensionId_ and _extensionVersionId_.
  * The steps below assume you already registed the sample echo extension and have your _extensionId_ and _extensionVersionId_.



## Executing extensions

The Draco [execution pipeline](https://github.com/microsoft/draco/blob/master/doc/architecture/execution-pipeline.md) defines how extensions are executed. For more information on the execution pipeline components, see the [pipeline](https://github.com/microsoft/draco/blob/master/doc/architecture/execution-pipeline.md) architecture and the [execution API](https://github.com/microsoft/draco/blob/master/src/draco/api/Execution.Api).

After you [register](Register-Extension.md) an extension, client applications must call the [execution API](https://github.com/microsoft/draco/blob/master/src/draco/api/Execution.Api) to execute the extension.

* Replace {{execution_url}} with the execution URL from the setup scripts.  This will be `http://draco-***-apim.azure-api.net/execution`.
* Replace {{extensionId}} with the _extensionId_ obtained when registering the extension.
* Replace {{extensionVersionId}} with the _extensionVersionId_ obtained when registering the extension.


```json
POST http://{{execution_url}}/execution

Request Headers:
    Content-Type: application/json

JSON payload:
{
	"extensionId": "{{extensionId}}",
	"extensionVersionId": "{{extensionVersionId}}",
	"requestData": {
        "message" : "hello, world!"
     }
}
```

After you execute your extension, the response is returned to you through the execution pipeline. For the sample echo extension, this response includes the payload you originally sent to the extension:

```json
    {
        "requestData": {
            "request": "some request data"
            },
            "properties": {
                "message": "hello, world!"
            }
        }
    }
```
