# Draco FAQ

This FAQ answers some of the questions you may have about Draco

---

## Q: How can I enable or disable an extension in Draco? 

  A: Draco checks whether an extension is enabled or not in the [ExecutionRequestContextBuilder](https://github.com/microsoft/draco/blob/master/src/draco/api/Execution.Api/Services/ExecutionRequestContextBuilder.cs).  Specifically, it checks for `ExtensionVersion.IsActive`.   

[ExtensionManagement Api](https://github.com/microsoft/draco/tree/master/src/draco/api/ExtensionManagement.Api) is where extensions are created, deleted and enabled, see [ExtensionController.cs](https://github.com/microsoft/draco/blob/master/src/draco/api/ExtensionManagement.Api/Controllers/ExtensionController.cs).



## Q:  How can I setup the Draco platform in Azure? 

A:  Setting up the Draco platform in Azure is documented in [draco/doc/setup](https://github.com/microsoft/draco/tree/master/doc/setup)  


