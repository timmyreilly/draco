# Prioritized execution

Prioritized execution enables ISVs to build a flexible sales model around any extension.

![Prioritized execution](/doc/images/arch-execution-priority_LI.jpg)

## Overview

When creating a new execution, customers have the ability to select one of three available execution priorities – **low**, **normal** (default), or **high**. For any given extension, supported priorities are defined as part of the **execution profile**.

The **low**, **normal**, and **high** execution priority paths are all asynchronous. For short-running extensions, you may choose to expose the extension synchronously (the **immediate** path) at a premium for customers that need immediate results.

Typically, the selected priority affects both the amount of time that the execution takes to complete as well as some cost factor. For example, a **normal-priority** execution may take about 10 minutes and cost $1.00 while a **high-priority** execution of the same extension may take about 5 minutes and cost $2.00.

The extension is the same regardless of the execution priority path. This is a great example of how a single extension can be broken down into multiple offerings based on customer need. An ISV can start small by offering only **normal-priority** execution then add additional priorities as customer needs evolve.

> **Best practice:** Use the [execution events emitted by the execution API](overview.md#execution-events) to automatically calculate estimated execution times based on priority. This information can be shared directly with customers and used to dynamically adjust pricing based on demand.

> **Best practice:** Using [Azure Spot VMs](https://docs.microsoft.com/en-us/azure/virtual-machines/windows/spot-vms) allows you to take advantage of unused capacity at a significant cost savings. At any point in time when Azure needs the capacity back, the Azure infrastructure will evict Spot VMs. Therefore, Spot VMs are great for **low-priority** extension execution. In this model, the Draco execution agent is pre-installed and configured on the Spot VM image. When the target price meets or falls below the current price, Spot VMs spin up and immediately begin processing **low-priority** execution requests. Note that, due to their ephemeral nature, Spot VMs do not offer an SLA.
