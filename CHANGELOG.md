# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

This project adheres to Azure [API Versioning](https://docs.microsoft.com/en-us/azure/api-management/api-management-versions) with [Revisions](https://docs.microsoft.com/en-us/azure/api-management/api-management-revisions) enabled.

Each version and revision is followed by a date of change, specially for under development versions. Each entry provides the following information (when applicable):

- **Breaking Changes**: changes that may certainty affect consumers of the API or how it is expected to be used.
- **Major Changes**: big improvements in the code, like adding or enabling features, or bug fixes.
- **Minor Changes**: small changes that have little impact, like spell checks in an API's documentation, adding or removing comments, etc.

Also, any bug fix must start with the prefix «Bug fix:» followed by the description of the changes _per se_.

Previous classification is not required if changes are simple or all belong to the same category.

## [6.0.4]

### Important

This version updates the `Semantic Kernel` library to version `1.0.0-beta8`, which introduces a lot of breaking changes in the code that mostly translate into multiple obsolescence warnings. Eventually, newer versions of this library will fix these warnings once a final version of `Semantic Kernel` is used.

The main motivation for this update is to take advantage of the latest improvements in the `Semantic Kernel` library, like the `Stepwise Planner` or Function Calls, plus better integrations with LLMs like OpenAI, among many other improvements.

Sadly, some warnings regarding types or members obsolescence could not be addresses until the Microsoft team behind `Semantic Kernel` provides a final version of the library. So far, these warnings are:

 - CS0618: *IKernel.PromptTemplateEngine' is obsolete: 'PromptTemplateEngine has been replaced with PromptTemplateFactory and will be null. If you pass an PromptTemplateEngine instance when creating a Kernel it will be wrapped in an instance of IPromptTemplateFactory. This will be removed in a future release.*
 - CS0618: *ISKFunction.RequestSettings' is obsolete: 'Use PromptTemplateConfig.ModelSettings instead. This will be removed in a future release.*
 - CS0618: *ISKFunction.SkillName' is obsolete: 'Methods, properties and classes which include Skill in the name have been renamed. Use ISKFunction.SkillName instead. This will be removed in a future release.*
 - CS0618: *PromptTemplate' is obsolete: 'IPromptTemplateEngine is being replaced with IPromptTemplateFactory. This will be removed in a future release.*

### Breaking Changes

- `Encamina.Enmarcha.SemanticKernel.Abstractions`
  * Removed method `ValidateAndThrowIfErrorOccurred`.
  * Removed properties `ChatModelName`, `CompletionsModelName`, and `EmbeddingsModelName` from `SemanticKernelOptions`.
- The following methods do not throw an `ArgumentException` if the instance of `ISKFunction` is not a semantic function, since now `Semantic Kernel` does not longer differentiates between Semantic and Native functions:
  * GetSemanticFunctionPromptAsync
  * GetSemanticFunctionUsedTokensAsync

### Major Changes

### Minor Changes
- Renamed sample projects to match Microsoft's naming conventions.
- Sample projects also use the new `Semantic Kernel` library version `1.0.0-beta8`.
- Some boy scouting by editing the comments in the code to have correct grammar and fixing some StyleCop warnings.

## [6.0.3.20]

### **Major Changes**

- Added  this `CHANGELOG.md`
- In `Encamina.Enmarcha.SemanticKernel.Abstractions`, method `ValidateAndThrowIfErrorOccurred` is now obsolete, and will be removed in a future version of this library.
- In `Encamina.Enmarcha.SemanticKernel.Plugins.Chat`, property `ChatRequestSettings` is now obsolete due to future changes in Semantic Kernel library its type `ChatRequestSettings` will change to `OpenAIRequestSettings `. Therefore, the signature of this property will change in future versions of this library.
- In `Encamina.Enmarcha.SemanticKernel.Connectors.Memory`, the constructor of `MemoryQueryPlugin` is now obsolete due to future changes in Semantic Kernel library, where the semantic memory will be a dependency outside the `IKernel`. The `IKernel` dependency will be replaced with `ISemanticTextMemory`. The signature of this constructor will change in future versions of this library. 
- In `Encamina.Enmarcha.SemanticKernel.Connectors.Memory`, the extension method `ImportMemoryPlugin` is now obsolete due to future changes in Semantic Kernel library, where the semantic memory will be a dependency outside the `IKernel`. An additional dependency with `ISemanticTextMemory` will be added to this extension method. The signature of this method will change in future versions of this library.

## [6.0.3.18] and [6.0.3.19]

- First Open Source versions of ENMARCHA.