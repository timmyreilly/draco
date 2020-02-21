# Extension services

Draco injects independently named and versioned **extension services** into extensions that support them at execution time.

![Extension services](/doc/images/arch-extension-services.JPG)

## Notes

- Extensions advertise the services that they support to the catalog.
- Extension services can be nearly anything.
  - Centralized logging/monitoring services like Application Insights and Splunk can be exposed as services.
  - Client hardware like printers, scanners, etc. can be exposed as services.
  - APIs, configuration information, and secrets can also be shared with an extension through services.
