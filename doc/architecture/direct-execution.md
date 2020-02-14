# Direct execution

In cases where sub-second response times are needed and/or Internet connectivity is limited, extensions can be executed in **direct mode**.

![Direct execution](/doc/images/arch-direct-execution.JPG)

## Notes

- In **direct mode**, catalogs issue clients digitally-signed, time-limited tokens that can be used to execute extensions directly and repeatedly.
- Extensions use the catalog’s public key to guarantee the authenticity of the execution token and its metadata.
