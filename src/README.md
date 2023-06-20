# Prompt / Native Code Chaining

A simplified c# end-to-end sample to chain semantic functions and native functions.

## Folder

| Folder | Content | Description |
| --- | --- | --- |
| Client | .NET application using Semantic Kernel to chain semantic functions (prompts) with native functions (.NET code) |  |
| CreateEnv | Azure CLI script to create the necessary environment to run the sample application | The Azure CLI script provides the credentials (Azure API Key, Azure Endpoint, Azure Deployment Name) used in the sample application |
| SkillsLibrary | Semantic functions and native functions used in the sample application | Both semantic and native functions are grouped into Skills where every semantic skill has it's own folder. The native JsonSkill is provided as a .NET Class Library (classlib)  |




