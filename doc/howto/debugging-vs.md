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

| Instructions | Screenshot |
| ------------ | ---------- |
| Navigate to the Azure portal. | ![Azure portal](/doc/images/debug-portal.JPG) |
| Navigate to the Draco platform resource group. By default, the name of this resource group is `draco-platform-rg` unless you explicitly chose a different one during the initial [setup](/doc/setup) process. | |
| Locate the Draco platform storage account within the resource group. The name of your storage account won't be the same as that shown in the screenshot but, by default, it will be the only storage account in the resource group. | |
