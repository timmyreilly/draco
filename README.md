![Draco](doc/images/draco.128.png)
  
[![Build Status](https://dev.azure.com/ms/Draco/_apis/build/status/ci-infra-exthub?branchName=master)](https://dev.azure.com/ms/Draco/_build/latest?definitionId=320&branchName=master)

---

# Draco

> **Draco is currently in preview. We do not _yet_ recommend it for production use.**

Draco is a set of open source services that make it easy to deploy, serve, and manage trusted first and third-party application [extensions](/doc/README.md#extensions), securely and at scale, anywhere.

Our vision for Draco is to enable all [independent software vendors (ISVs)](https://blogs.partner.microsoft.com/mpn-canada/whats-isv-faq-microsoft-partners-independent-software-vendors-app-builders/), regardless of industry or size, to extend their existing product offerings into the cloud. We see Draco as an on-ramp to delivering [software as a service (SaaS)](https://azure.microsoft.com/en-us/overview/what-is-saas/) through extensions.

See our [architecture docs](/doc/README.md) for more information on how Draco works and some background on its design decisions.

## Getting started

Draco is a container-based platform intended to be run "out-of-the-box" on the [Microsoft Azure](https://azure.microsoft.com) cloud. [Scripts and templates are available](/src/draco/infra) to deploy the Azure infrastructure, build and deploy the Draco services, and validate the environment is running properly. To get started, see the [setup instructions](https://github.com/microsoft/draco/blob/master/doc/setup/README.md) to deploy Draco in your Azure subscription. 

Refer to our [FAQ](/doc/FAQ.md) for answers to some common questions about Draco.

## Open source and our partner-driven philosophy

We know that we can't deliver on Draco's vision alone. Microsoft has always relied on partners to help set direction and drive requirements based in the real world. We need partners to help us build features and guidance that meet their unique needs.

We firmly believe in transparency and that the best solutions are built in the open with support and contributions from our partners and the open source community at large. In this spirit, we have decided to release Draco to open source early as a preview. It is our hope that, by doing so, we can accelerate the delivery of a truly production-ready solution.

> While the platform today enables most key ISV scenarios, there are gaps, tagged in our backlog as ["help wanted"](https://github.com/microsoft/draco/labels/help%20wanted), that we are looking to our partner community to help address.

## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
