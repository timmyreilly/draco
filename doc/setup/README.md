# Setup the Draco platform components

The following instructions are how to build and setup the Draco platform components.  These components are required to build an extension, but many coporate developers will probably be given an environment to use (check with your lead).

See [Draco Architecture](../architecture/azure-architecture.md) for more information.

> The following instructions have been tested and verified using Ubuntu 18.04 and MacOS Catalina 10.15

## Pre-requisites

* Azure Subscription
  * You'll also need your subscription ID. You can find this at https://ms.portal.azure.com/#blade/Microsoft_Azure_Billing/SubscriptionsBlade.
* Permission to create a service principal (SPN) in Azure AD
* [Kubernetes CLI](https://kubernetes.io/docs/tasks/tools/install-kubectl/)
* [Docker](https://www.docker.com/products/docker-desktop)
* [Helm](https://github.com/helm/helm/releases)
* [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
* [Azure Kubernetes CLI / kubectl](https://docs.microsoft.com/en-us/cli/azure/aks?view=azure-cli-latest#az-aks-install-cli)

## Specific to MacOS

* [Home Brew](https://brew.sh/) to install the following tools:
  * azure-cli (alternative install method to direct download)
  * docker  (You still need Docker Desktop from above)
  * kubernetes-cli
  * helm

## Login to Azure Subscription

Use the following commands to authenticate to Azure and set the Azure subscription you want Draco deployed in.

```bash
az login

# If you have multiple subscriptions, set the subscription to use for Draco.
az account list 
az account set --subscription "Name of Subscription"
```

## Run the common resources setup script

This section sets up all the common resources (Key Vault, Container Registry, API Management) that will be shared across Draco platform deployments. Follow these instructions only if you have not already set up the common Draco resource group (by default `draco-commong-rg`). If you already have, feel free to [skip to the next section](#run-the-platform-resources-setup-script).

> This section takes approximately 35 minutes.

### Instructions

1. Starting from your local Draco repository root, run the following command:

   ```
   cd src/draco/infra
   ```
   
2. Execute the common resource setup script. From the command line, run...
   
   ```bash
   ./setup-common.sh -l "[Azure region name]" \
                     -n "[AKS service principal name]"
   ```
   
   * Replace `[Azure region name]` with the name of the Azure region that you would like to deploy Draco to. For example, if you want to deploy Draco the Central US region, replace `[Azure region name]` with `centralus`. For a complete list of Azure location names, run `az account list-locations -o table` from the command line.
   
   * Replace `[AKS service principal name]` with the an arbitrary, unique Azure Active Directory service principal name. We recommend that you adhere to this format: `https://k8s-draco-[some unique value]`.
   
      > **Note**: The `[AKS service principal name]` needs to be unique to the Azure Active Directory (AAD) tenant protecting your target subscription.  If you have other deployments of this platform in subscriptions protected by the same AAD tenant, then change the value to something that doesn't currently exist in your tenant.    
   
   * For complete usage information and to see additional optional parameters available, run `./setup-common.sh`.
  
3. When the script finishes, it will output information needed in the next section similar to that below. Be sure to note these values somewhere.

   ```bash
   Common Resource Group Name:     [draco-common-rg]
   AKS Service Principal ID:       [{guid}]
   ```

## Run the platform resources setup script

This section sets up all the resources needed to host a Draco platform. This section expects that you have already [set up the Draco common resources](#run-the-common-resources-setup-script) and obtained the following values:

* `Common Resource Group Name` (default is `draco-common-rg`)
* `AKS Service Principal ID`

> This section takes approximately 25 minutes.

### Instructions

1. Execute the platform resource setup script. From the command line, run the following command:

   > Note: The _platform_ resources will deploy to the same region the _common_ resources are deployed to, but in a different resource group.  The resource group name defaults to `draco-platform-rg` but can be changed using the `-r` parameter.
   
   ```bash
   ./setup-platform.sh -a "[AKS Service Principal ID]" \
                       -c "[Common Resource Group Name]"
   ```
      
      * Replace `[AKS Service Principal ID]` with the value returned from the `setup-common.sh` script in the [common setup section](#run-the-common-resources-setup-script).
     
      * For complete usage information and to see additional optional parameters available, run `./setup-platform.sh`.
   
2. Verify that the Draco Kubernetes pods are running using `kubectl get pods`. Your output should look similiar to that below.

   ```bash
   NAME                           READY   STATUS    RESTARTS   AGE
   initial-catalogapi-...         1/1     Running   0          19s
   initial-catalogapi-...         1/1     Running   0          19s
   initial-catalogapi-...         1/1     Running   0          19s
   initial-executionapi-...       1/1     Running   0          19s
   initial-executionapi-...       1/1     Running   0          19s
   initial-executionapi-...       1/1     Running   0          19s
   initial-executionconsole-...   1/1     Running   0          19s
   initial-executionconsole-...   1/1     Running   0          19s
   initial-executionconsole-...   1/1     Running   0          19s
   initial-extensionmgmtapi-...   1/1     Running   0          19s
   initial-extensionmgmtapi-...   1/1     Running   0          19s
   initial-extensionmgmtapi-...   1/1     Running   0          19s
   ```
   
   > **Note**: There are three copies of each of the four core Draco services running in your AKS cluster. For high availability, [the Helm chart](/src/draco/helm/extension-hubs) included with Draco specifies that each service should have three replicas running at all times.  
 
3. Verify the internal load balancers for the Draco services are configured by running `kubectl get services`. Your output will look similar to that below.  After the `EXTERNAL-IP`s are available (ie: not `<pending>`) you can continue to the next section.

   ```bash
   NAME                       TYPE           CLUSTER-IP     EXTERNAL-IP   PORT(S)                      AGE
   initial-catalogapi         LoadBalancer   10.0.###.###   <pending>     80:#####/TCP,443:#####/TCP   119s
   initial-executionapi       LoadBalancer   10.0.###.###   <pending>     80:#####/TCP,443:#####/TCP   119s
   initial-extensionmgmtapi   LoadBalancer   10.0.###.###   <pending>     80:#####/TCP,443:#####/TCP   119s
   ```

**Don't continue to the next section until _after_ the `EXTERNAL-IP`s are available.  It generally takes 1-2 mins for the internal load balancers to configure.**

## Run the Draco API configuration script

This section configures the Draco API's in the Azure API Management instance in the common resource group. This section expects that you have already [set up the Draco common resources](#run-the-common-resources-setup-script) and [setup the Draco platform resources](#run-the-platform-resources-setup-script).

> This section takes approximately 1 minute.

### Instructions

1. Execute the Draco API configuration script. From the command line, run the following command:

   ```bash
   ./setup-draco-apis.sh -c "[Common Resource Group Name]"
   ```

2. When the script finishes it will output the URL for the Draco API's.  These URL's may be used to call the API's to [register an extension](/doc/howto/Register-Extension.md), [search for extensions](/doc/howto/search-extension.md), and [execute an extension](/doc/howto/Execute-Extension.md).

## Register your first extension (optional)
Now the Draco infrastrucutre is ready to use.  Next, consider [registering a sample extension](/doc/howto/Register-Extension.md).

## Uninstalling Draco
For more information on uninstalling Draco, see [this guide](UNINSTALL.md).
