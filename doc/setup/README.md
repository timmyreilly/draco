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
  
## Run the common resources setup script

This section sets up all the common resources (Azure Key Vault and Azure Container Registry) that will be shared across Draco platform deployments. Follow these instructions only if you have not already set up the common Draco resource group (by default `draco-commong-rg`). If you already have, feel free to [skip to the next section](#run-the-platform-resources-setup-script).

> This section takes approximately 15 minutes.

### Instructions

1. Starting from your local Draco repository root, navigate to the common resources setup script directory. 
   
   * From the command line, run `cd src/draco/infra/ArmTemplate/common`.
   
2. Execute the common resource setup script. From the command line, run...
   
   ```bash
   ./setup-common.sh -l "[Azure region name]" \
                     -n "[AKS service principal name]" \
                     -s "[Your Subscription ID]"
   ```
   
   * Replace `[Azure region name]` with the name of the Azure region that you would like to deploy Draco to. For example, if you want to deploy Draco the Central US region, replace `[Azure region name]` with `centralus`. For a complete list of Azure location names, run `az account list-locations -o table` from the command line.
   
   * Replace `[AKS service principal name]` with the an arbitrary, unique Azure Active Directory service principal name. We recommend that you adhere to this format: `https://k8s-draco-[some unique value]`.
   
      > **Note**: The `[AKS service principal name]` needs to be unique to the Azure Active Directory (AAD) tenant protecting your target subscription.  If you have other deployments of this platform in subscriptions protected by the same AAD tenant, then change the value to something that doesn't currently exist in your tenant.    
   
   * For complete usage information, run `./setup-common.sh`.
  
3. If the common resource script executes successfully, you will be presented with a table containing important information needed in the next section similar to that below. Be sure to note these values somewhere.

   ```bash
   Common Resource Group Name:     [draco-common-rg]
   Deployment Name:                [draco-common-...]
   Container Registry ID:          [/subscriptions/...]
   Container Registry Name:        [draco...]
   Key Vault Name:                 [draco...]
   AKS Service Principal ID:       [...]
   AKS Service Principal Password: [...]
   ```

## Run the platform resources setup script

This section sets up all the resources needed to host a Draco platform. This section expects that you have already [set up the needed Draco common resources](#run-the-common-resources-setup-script) and obtained the following values -

* `Common Resource Group Name` (default is `draco-common-rg`)
* `Container Registry Name`
* `Key Vault Name`
* `AKS Service Principal ID`

> This section takes approximately 45 minutes.

### Instructions

1. Starting from your local Draco repository root, navigate to the platform resources setup script directory. 
   
   * From the command line, run `cd src/draco/infra/ArmTemplate/exthub`.
   
2. Execute the platform resource setup script. From the command line, run...
   
   ```bash
   ./setup-platform.sh -l "[Azure region name]" \
                       -a "[AKS Service Principal ID]" \
                       -c "[Common Resource Group Name]" \
                       -k "[Key Vault Name]" \
                       -r "[Container Registry Name]" \
                       -s "[Subscription ID]"
   ```
   
   * Replace `[Azure region name]` with the name of the Azure region that you would like to deploy Draco to. For example, if you want to deploy Draco the Central US region, replace `[Azure region name]` with `centralus`. For a complete list of Azure location names, run `az account list-locations -o table` from the command line.
     
   * Replace `[AKS Service Principal ID]`, `[Key Vault Name]`, and `[Container Registry Name]` with the values returned from the `setup-common.sh` script in the [common setup section](#run-the-common-resources-setup-script).
     
   * For complete usage information, run `./setup-platform.sh`.
   
3. Make a note of the Draco endpoint URLs provided when the script is done running. Once all the Kubernetes services/load balancers are provisioned (see step 6), you should be able to access these Draco APIs. Your output should look similiar to that below.

   ```bash
   Draco Catalog API:               [http://draco-...-catalogapi...cloudapp.azure.com]
   Draco Execution API:             [http://draco-...-executionapi...cloudapp.azure.com]
   Draco Extension Management API:  [http://draco-...-extensionmgmtapi...cloudapp.azure.com]
   ```
     
4. Verify that the Draco Kubernetes pods have been deployed by running `kubectl get pods`. Your output should look similiar to that below.

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
   
   > **Note**: You may notice that there are three copies of each of the four core Draco services running in your AKS cluster. For high availability, [the Helm chart](/src/draco/helm/extension-hubs) included with Draco specifies that each of the four core services should have three replicas running at all times.  
 
5. Verify that the Draco Kubernetes services/public load balancers have been deployed by running `kubectl get services --watch`. Your output should look similar to that below. Once `EXTERNAL-IP`s are available and no longer in a `<pending>` state for each service, you should be able to access the Draco APIs.

   ```bash
   NAME                       TYPE           CLUSTER-IP     EXTERNAL-IP   PORT(S)                      AGE
   initial-catalogapi         LoadBalancer   10.0.###.###   <pending>     80:#####/TCP,443:#####/TCP   119s
   initial-executionapi       LoadBalancer   10.0.###.###   <pending>     80:#####/TCP,443:#####/TCP   119s
   initial-extensionmgmtapi   LoadBalancer   10.0.###.###   <pending>     80:#####/TCP,443:#####/TCP   119s
   ```

## Register your first extension
At this point, all of your Draco infrastrucutre should be stood up and ready to use. Next, you should [try registering your first extension](/doc/howto/Register-Extension.md).

## Uninstalling Draco
For more information on uninstalling Draco, see [this guide](UNINSTALL.md).
