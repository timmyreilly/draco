# Execution models

An **execution model** describes *how* an extension is executed.

![Execution models](/doc/images/arch-execution-models.JPG)

## Overview

The **execution model** is the core architectural concept that enables the nearly unlimited flexibility of *how* extensions are executed. As you can see in the above diagram, **execution models** are both uniquely identifable and independently versioned (e.g., **http/v1** and **other/v1**). 

Each **execution model** has a corresponding **execution adapter** that decouples the underlying Draco control plane from the extensions themselves. **Execution adapters** can be registered [directly through the execution API](/src/Execution.Api/Modules/Factories/ExecutionProcessorFactoryModule.cs) to enable synchronous execution or [through the stand-alone execution agent](/src/ExecutionAdapter.ConsoleHost/Modules/ExecutionProcessorFactoryModule.cs) to enable asynchonous execution.

> **Best practice**: To enable even looser coupling and frictionless, zero-downtime updates, Draco provides a [standardized mechanism for wrapping **execution adapters** in their own APIs](/src/ExecutionAdapter.Api) which can then be deployed independently of the execution API and/or execution console. This also makes it easier to create **execution adapters** in cases where C# is not the preferred programming language.

> **Best practice**: In cases where the control plane resides in Azure but asynchronous extension execution is required on-premises, consider deploying the execution agent on-premises as well. By default, asynchronous execution requests are published to an [Azure service bus topic](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-queues-topics-subscriptions#topics-and-subscriptions). The on-premises execution agent can subscribe to the topic. From a networking security perspective, this configuration requires only that the on-premises site allow TCP port 443 outbound.

## Implementation

Under the hood, **execution adapters** implement the [IExecutionAdapter interface](src/Core.Execution/Interfaces/IExecutionAdapter.cs). If you're creating new **execution adapters** in C#, this is the recommended approach even if you're planning on [wrapping the **execution adapter** within its own API](/src/ExecutionAdapter.Api). Using a common interface gives you greater flexibility in how you deploy **execution adapters**.

## Example

Out of the box, Draco provides support for extensions that implement a simple JSON-based REST API via the **http/v1 execution model** supported by [this **execution adapter**](/src/Core.Execution/Adapters/JsonHttpExecutionAdapter.cs). The **http/v1 execution model** expects extensions to expose a single HTTP POST endpoint that accepts [this JSON request](/src/Core.Execution/Models/HttpExecutionRequest.cs) and returns [this JSON response](/src/Core.Execution/Models/HttpExecutionResponse.cs).

For more information on how [the **http/v1 execution adapter**](/src/Core.Execution/Adapters/JsonHttpExecutionAdapter.cs) is registered, see –

- **Best practice**: [Execution adapter API registration](/src/ExecutionAdapter.Api/Modules/Factories/ExecutionProcessorFactoryModule.cs)
- [Execution API registration](/src/Execution.Api/Modules/Factories/ExecutionProcessorFactoryModule.cs)
- [Execution console registration](/src/ExecutionAdapter.ConsoleHost/Modules/ExecutionProcessorFactoryModule.cs)
