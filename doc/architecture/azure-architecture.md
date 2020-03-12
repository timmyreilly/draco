# Azure architecture overview

Out of the box, Draco includes everything you need to get up and running on [Microsoft Azure](https://azure.microsoft.com).

---

![Architecture overview](/doc/images/arch-azure.JPG)

## Architecture diagram notes

The notes below refer to the red numbers in the architecture diagram above.

1. All core Draco services are packaged into [Docker containers](https://www.docker.com/resources/what-container). On Azure, these containers are deployed into an [Azure Kubernetes Service (AKS) cluster](https://azure.microsoft.com/en-us/services/kubernetes-service/) using a [Helm chart](/src/draco/Helm/extension-hubs) to deploy all the services that you need to get started in your AKS cluster.
2. Container images are stored in an [Azure Container Registry](https://azure.microsoft.com/en-us/services/container-registry/). The AKS cluster pulls container images from the container registry using an [Azure Active Directory](https://azure.microsoft.com/en-us/services/active-directory) service principal.
3. Draco APIs are protected by Azure Active Directory, requiring the client to be authenticated.
4. Draco's catalog API is powered by [Azure Search](https://azure.microsoft.com/en-us/services/search/) enabling full-text search capability over the entire extension catalog. Azure Search automatically indexes extension metadata stored in [Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/) (note 5).
5. By default, **[execution objects](execution-objects.md)** are stored in [Azure blob storage](https://azure.microsoft.com/en-us/services/storage/blobs) using the **az-blobs/v1 [object provider](/doc/architecture/execution-objects.md#object-providers)**. Draco service configuration, stored as JSON documents, is also stored in [Azure blob storage](https://azure.microsoft.com/en-us/services/storage/blobs).
6. All extension and execution metadata are stored as JSON documents in [Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/).
7. When an execution changes state (an execution request has been queued, is in progress, has succeeded or failed, etc.), [an event is published](overview.md#execution-events) by the execution API to an [Azure Event Grid](https://azure.microsoft.com/en-us/services/event-grid/) [topic](https://docs.microsoft.com/en-us/azure/event-grid/concepts#topics). These events can be [subscribed to](https://docs.microsoft.com/en-us/azure/event-grid/concepts#event-subscriptions) and used to add additional value-added capabilities such as analytics and business intelligence, user notifications, and infrastructure autoscaling.
8. Inter-API communication is enabled through Kubernetes [services](https://kubernetes.io/docs/tutorials/services/) and [DNS](https://kubernetes.io/docs/concepts/services-networking/dns-pod-service/) and secured using internal [Azure Active Directory](https://azure.microsoft.com/en-us/services/active-directory) [service principals](https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-client-creds-grant-flow).
9. To enable asynchronous execution, the execution API publishes execution requests to an [Azure service bus](https://docs.microsoft.com/en-us/azure/service-bus-messaging/) [topic](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-queues-topics-subscriptions#topics-and-subscriptions). Each execution request is tagged with routing metadata including the **[execution model](execution-models.md)**, **[execution priority](prioritized-execution.md)**, and **execution profile**. This metadata is used to route the execution requests to subscribed execution agents that can handle them. This architecture allows execution requests submitted to a common topic to be automatically distributed to the right execution agent regardless of where it is running.
