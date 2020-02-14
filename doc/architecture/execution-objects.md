# Execution objects

**Execution objects** allow clients and extensions to efficiently and securely exchange binary objects (files).

![Execution objects](/doc/images/arch-execution-objects.JPG)

## Notes

- Extension Hub’s unique, pluggable **object provider** model makes it easy to implement custom object storage – SMB, NFS, AWS S3, or any other storage provider.
- Like **execution models**, **object providers** are independently named and versioned.
- Out of the box, Extension Hub natively supports Azure blobs (**azblobs/v1**).
