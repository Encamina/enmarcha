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

This version updates the `Semantic Kernel` library to version `1.0.0-beta8`, which introduces a lot of breaking changes in the code that mostly translate in multiple obsolete warnings. Eventually, newer versions of this library will fix these obsolescences once a final version of `Semantic Kernel` is used.

The main motivation for this update is to take advantage of the latest improvements in the `Semantic Kernel` library, like the `Stepwise Planner` or Function Calls, plus better integrations with LLMs like OpenAI, among many other improvements.

### Breaking Changes

- `Encamina.Enmarcha.SemanticKernel.Abstractions`
  * Removed method `ValidateAndThrowIfErrorOccurred`.
  * Removed properties `ChatModelName`, `CompletionsModelName`, and `EmbeddingsModelName` from `SemanticKernelOptions`.
- The following methods do not throw an `ArgumentException` if the instance of `ISKFunction` is not a semantic function, since now `Semantic Kernel` does not longer differentiates between Semantic and Native functions:
  * GetSemanticFunctionPromptAsync
  * GetSemanticFunctionUsedTokensAsync
  

### **Major Changes**

#### `Encamina.Enmarcha.SemanticKernel.Abstractions`
  - Implementation of `LengthByTokenCount` now uses `SharpToken` instead of `GPT3Tokenizer` which has been removed from `Semantic Kernel`. The count of tokens using the `SharpToken` library remains the same as before. Reference: 
    * https://github.com/microsoft/semantic-kernel/issues/2508
    * https://devblogs.microsoft.com/semantic-kernel/introducing-the-v1-0-0-beta1-for-the-net-semantic-kernel-sdk/
  - New implementation of `LengthByTokenCountUsingEncoding` to count tokens by a given encoding.

 ### **Major Changes**
  - Fixed some documentation typos and other boy scouting tasks.

## [6.0.3.19]

- First Open Source version of ENMARCHA.