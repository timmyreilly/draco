# Executing an extension from your application

The following instructions are how to execute a Draco platform extension. 

> The following instructions have been tested and verified using Ubuntu 18.04 

## Pre-requisites

* [Draco Environment Pre-Setup](https://github.com/microsoft/draco/blob/master/doc/setup/README.md)
* Review sample [echo extension](../../src/extensions/samples/csharp/netcore-simple/echo)
* Your extension is already [registered](Register-Extension.md) in the Draco platform. As part of the registration you should have an extension id, extension version id, as well as an execution url to call the registered extension.

## Executing extensions

The Draco [execution pipeline](https://github.com/microsoft/draco/blob/master/doc/architecture/execution-pipeline.md) defines how extensions are executed. For more information on the execution pipeline components, see the [pipeline](https://github.com/microsoft/draco/blob/master/doc/architecture/execution-pipeline.md) architecture and the [execution API](https://github.com/microsoft/draco/blob/master/src/draco/api/Execution.Api).

After you [register](Register-Extension.md) an extension, client applications must call the [execution API](https://github.com/microsoft/draco/blob/master/src/draco/api/Execution.Api) to execute the extension, using extensionId and the extensionVersionId that you will get from Draco after registering it:

```json
POST http://<execution_api_url>/execution
payload:
{
	"extensionId": "{{extensionId}}",
	"extensionVersionId": "{{extensionVersionId}}",
	"requestData": {
        "message" : "hello, world!"
     }
}
```

You can use [execution API](https://github.com/microsoft/draco/blob/master/src/draco/api/Execution.Api) url to call your extension.

## Example: executing echo extension

  Echo extension is implemented as a WebAPI and has a single POST method that accepts a JSON message and returns a response echoing the input it receives. You can find the extension code for [Echo](../../src/extensions/samples/csharp/netcore-simple/echo). 

 The previous section shows how to execute any extension.  For the sample echo extension included in Draco, you can follow the steps in [extension registration](Register-Extension.md) to obtain extensionId, extensionVersionId and external IP of the execution API. Note, that for echo extension, the name of the http route is echo:

```json
API Request
HTTP Verb and URL:
    Set Verb to POST
    Set URL to http://<execution_api_url>/execution

Request Headers:
    Content-Type: application/json

Request Payload 
Use this format:
   {
	    "extensionId": "{{extension_id}}",
	    "extensionVersionId": "{{extension_version_id}}",
	    "requestData": {
		    "request": "some request data"
	    },
	    "properties": {
		    "message": "hello, world!"
	    }
    }
```

After you execute your extension, the response is returned to you through the execution pipeline. For the sample echo extension, this response includes the payload you originally sent to the extension:

```json
    {
        "properties": {
            "message": "hello, world!"
        },
        "executionId": "842c10cf-7f32-419a-be96-2d59f3b183b4_c02d1efa-d4f6-4da1-81b2-69d780809d15_598f9af9-f226-41b4-bd82-a2944cb1f1b1_be63336f-2eee-492a-a4a2-f68b4363085a",
        "status": "Queued",
        "getExecutionStatusUrl": "http://20.186.44.156/execution/842c10cf-7f32-419a-be96-2d59f3b183b4_c02d1efa-d4f6-4da1-81b2-69d780809d15_598f9af9-f226-41b4-bd82-a2944cb1f1b1_be63336f-2eee-492a-a4a2-f68b4363085a/status",
        "putExecutionStatusUrl": "http://20.186.44.156/execution/842c10cf-7f32-419a-be96-2d59f3b183b4_c02d1efa-d4f6-4da1-81b2-69d780809d15_598f9af9-f226-41b4-bd82-a2944cb1f1b1_be63336f-2eee-492a-a4a2-f68b4363085a/status",
        "lastUpdatedUtc": "2020-04-02T19:47:27.1311593Z",
        "outputObjects": {}
    }
```

## For more information

See [how to register an extension](Register-Extension.md), [Draco architecture](../architecture/azure-architecture.md), [execution pipeline](https://github.com/microsoft/draco/blob/master/doc/architecture/execution-pipeline.md), [Execution API](https://github.com/microsoft/draco/blob/master/src/draco/api/Execution.Api) for more information.
