# Debugging Draco services locally

This document provides detailed guidance on debugging the core Draco services (API and remote execution agent) running in your Azure environment from your local developer machine using [Visual Studio 2019](https://visualstudio.microsoft.com/vs/).

> Documentation for other development environments (Visual Studio Code, etc.) is on the roadmap. We welcome contributions from the community in building this documentation out.

## Prerequisites

* A local [clone](https://help.github.com/en/github/creating-cloning-and-archiving-repositories/cloning-a-repository) of this Github repository
* A Draco environment running in your Azure subscription
  * For more information on setting up Draco, see [this document](/doc/setup).
* [Visual Studio 2019](https://visualstudio.microsoft.com/vs/)
  * If you don't already have Visual Studio 2019 installed, you can [download](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&rel=16) and use Visual Studio 2019 Community Edition for **free** for the purposes of debugging Draco. For more information on Community Edition license terms, see [this document](https://visualstudio.microsoft.com/license-terms/mlt031819/).
  
## Instructions

In order to debug Draco locally, you will first need to obtain an access key to the storage account that contains all of your configuration files. Next, you will need to open the Draco solution in Visual Studio, build it, point the service that you wish to debug to its configuration file in Azure storage, then debug just as you normally would locally. Detailed instructions are below.

### Get the storage account access key

| Instructions | Screenshot |
| ------------ | ---------- |
| Navigate to the Azure portal. | ![Azure portal](/doc/images/debug-portal.JPG) |
| Navigate to the Draco platform resource group.<br /><br />By default, the name of this resource group is `draco-platform-rg` unless you explicitly chose a different one during the initial [setup](/doc/setup) process. | ![Resource group](/doc/images/debug-rg.JPG) |
| Locate the Draco platform storage account within the resource group.<br /><br />Note that the name of your storage account won't be the same as that shown in the screenshot but, by default, it will be the only storage account in the resource group.<br /><br />Click on the storage account resource which should open up a pane similar to that in the screenshot.<br /><br />Locate and click on the `Access keys` setting highlighted in the screenshot. | ![Storage account](/doc/images/debug-storage.JPG) | 
| Locate and copy the storage account connection string highilghted in the screenshot. | ![Storage account connection string](/doc/images/debug-storage-keys.JPG)

### Debug in Visual Studio

| Instructions | Screenshot |
| ------------ | ---------- |
| Navigate to the root of your local Draco repository. From there, navigate to `/src/draco`. Open `Draco.sln`. Wait for the solution to load. | ![Draco.sln](/doc/images/debug-vs-sln.JPG) |
| Once the Draco solution has been loaded in Visual Studio, build it to ensure that everything works properly on your local machine.<br /><br />To build the solution, locate it in the Solution Explorer window (it should be at the top of the Solution Explorer window), right-click it, and choose `Rebuild Solution` as shown in the screenshot.<br /><br />Wait for the solution to build succesfully. | ![Build the solution](/doc/images/debug-vs-rebuild.JPG)
