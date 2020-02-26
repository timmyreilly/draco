# Architecture overview

Draco was designed from the ground up for extensibility, scalability, and security.

Although Draco includes built-in support for [hosting hubs completely in Azure](azure-architecture.md), it can easily be forked and modified to run on any other container-enabled platform – on-premises, at the intelligent edge, or even another cloud.

Before diving in, we recommend that you take a moment to familiarize yourself with [some basic Draco terminology](definitions.md).

![Architecture overview](/doc/images/arch-overview.JPG)

## Clients and APIs

As you can see in the diagram above, clients interact with Draco through a series of APIs. Draco itself does not provide a production-ready UI. This is very much by design. Typically, the extension user experience (UX) – from browsing the catalog to extension execution – should itself be a natural extension of the extended application. This approach also creates a clean separation of concerns that allows the UX to be updated independently of the underlying Draco platform and individual hub configuration.

## Identity

Draco is designed to integrate with identity providers that support OAuth 2.0 and OpenID Connect authentication flows. We plan on publishing sample code soon that demonstrates integration with common identity providers like Azure Active Directory and Salesforce. The goal is to allow hosts to bring their own identity providers enabling a seamless experience across existing applications and extensions.

## Extensions

Leveraging Draco's novel **[execution model-based](execution-models.md)** design, extensions can take nearly any shape or form including REST APIs, AI/ML models, third-party services, and shell scripts. Execution can be asynchronous, synchronous, or even **[direct](direct-execution.md)**. Draco also supports both short and long-running execution enabling both lightweight extensions that take advantage of shared, "always on" infrastructure and heavier extensions that rely on "just-in-time" infrastructure or even direct human intervention and workflows. Furthermore, these same extensions can be hosted anywhere – on Azure, on-premises, at the intelligent edge, or even on another cloud – with little to no modification.

Regardless of how they are executed or where they are hosted, all extensions share a centrally-managed Draco control plane that, itself, is designed to scale globally with zero downtime. This decoupling of extensions from the underlying control plane carries a wide range of additional benefits including the ability to leverage **[execution objects](execution-objects.md)** to efficiently and securely exchange files directly between clients and extensions regardless of the underlying storage platform.

The complexities of how and where extensions are executed is managed by the execution API. The execution API is itself extensible allowing hosts to plug in their own custom **[execution adapters](execution-models.md)**, **[extension services](extension-services.md)**, and **[object providers](execution-objects.md)** with little to no downtime.

## Execution events

Any time that an execution changes state (an execution request has been queued, is in progress, has succeeded or failed, etc.), an event is published that contains a wealth of information including –

- The execution ID
- The execution model, priority, and profile
- The extension and extension version ID
- The user and tenant ID
- An immutable, host-defined property bag associated with the execution
- UTC date/timestamps

These events can easily be consumed to create additional custom functionality including -

- Rich analytics and business intelligence
- Real-time execution time estimates
- Internal and external notifications
- Integration with existing billing systems
- Integration with existing CRM and marketing platforms