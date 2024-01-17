# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

This project adheres to Azure [API Versioning](https://docs.microsoft.com/en-us/azure/api-management/api-management-versions) with [Revisions](https://docs.microsoft.com/en-us/azure/api-management/api-management-revisions) enabled.

Each version and revision is followed by a date of change, specially for under development versions. Each entry provides the following information (when applicable):

- **Breaking Changes**: changes that may certainty affect consumers of the API or how it is expected to be used.
- **Major Changes**: big improvements in the code, like adding or enabling features, or bug fixes.
- **Minor Changes**: small changes that have little impact, like spell checks in an API's documentation, adding or removing comments, etc.

Also, any bug fix must start with the prefix �Bug fix:� followed by the description of the changes _per se_.

Previous classification is not required if changes are simple or all belong to the same category.

## [8.1.0]

### Important

This version updates the `Semantic Kernel` library from version `1.0.0-beta8` to `1.1.0`, which introduces a lot of breaking changes in the code.

Sadly, some features from `Semantic Kernel` that we might have been using, are marked as ***experimental*** and produce warnings that do not allow the compilation of the code. To use these features, these warnings must be ignored explicitly per project. The following is a list of these warnings and the affected projects:

 - SKEXP0001: 
   - `Encamina.Enmarcha.SemanticKernel`   
 - SKEXP0003: 
   - `Encamina.Enmarcha.SemanticKernel`
   - `Encamina.Enmarcha.SemanticKernel.Abstractions`
   - `Encamina.Enmarcha.SemanticKernel.Connectors.Memory`
   - `Encamina.Enmarcha.SemanticKernel.Plugins.Memory`
   - `Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering`
 - SKEXP0011:
   - `Encamina.Enmarcha.SemanticKernel.Connectors.Memory`
 - SKEXP0026: 
   - `Encamina.Enmarcha.SemanticKernel.Connectors.Memory`
 - SKEXP0051:
   - `Encamina.Enmarcha.SemanticKernel.Connectors.Document`

 More information about these warnings is available here: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/docs/EXPERIMENTS.md

### Braking Changes

- Replaced class `Completition` for `Completion` in `Encamina.Enmarcha.AI.OpenAI.Abstractions`. It was misspelled.
- Class `SemanticKernelOptions` does not exists anymore. It has been replaced by `AzureOpenAIOptions` from `Encamina.Enmarcha.AI.OpenAI.Abstractions`. 
- The following references were updated due to changes in `Semantic Kernel` version `1.0.1`:
 - Changed `IKernel` for `Kernel`.
 - Changed `ISKFunction` for `KernelFunction` or `KernelPlugin`.
 - Changed `SKFunction` for `KernelFunction`.
 - Changed `ContextVariables` for `KernelArguments`.
 - Changed `kernel.Functions.GetFunction(...)` for `kernel.Plugins[<name of plugin>][<name of function>]`.
 - Changed `OpenAIRequestSettings` for `OpenAIPromptExecutionSettings`.
- Removed extension methods for `SKContext` because that class does not exists anymore in `Semantic Kernel`.
- Due to the breaking nature of the new version of `Semantic Kernel`, the following extension methods are not available any more and have been replace by new methods, and it was not possible to marked it as `Obsolete`:
  - `GetSemanticFunctionPromptAsync` is replaced by `GetKernelFunctionPromptAsync`.
  - `GetSemanticFunctionUsedTokensAsync` is replaced by `GetKernelFunctionUsedTokensAsync`.
  - `ImportSemanticPluginsFromAssembly` is replaced by `ImportPromptFunctionsFromAssembly`.
- Extension method `GetSemanticFunctionPromptAsync` is no longer available. It is replaced by `GetKernelFunctionPromptAsync`. 
- Extension method `ImportQuestionAnsweringPlugin` has different signature.
- Extension method `ImportQuestionAnsweringPluginWithMemory` has different signature.
- Extension method `ImportChatWithHistoryPluginUsingCosmosDb` has different signature.
- The format of prompt function configuration files `config.json` has been modified.

### Major Changes

- Updated `Semantic Kernel` from `1.0.0-beta8` to `1.1.0` (second final version of `Semantic Kernel`).
- Updated `Azure.Core` from version `1.36.0` to `1.37.0`.
- Updated `Azure.AI.OpenAI` from version `1.0.0-beta.6` to `1.0.0-beta.12`.
- Updated `Bogus` from version `34.0.2` to `35.4.0`.
- Updated `Microsoft.AspNetCore.Authentication.JwtBearer` from version `8.0.0` to `8.0.1`.
- Updated `Microsoft.AspNetCore.Authentication.OpenIdConnect` from version `8.0.0` to `8.0.1`.
- Updated `Microsoft.Azure.Cosmos` from version `3.37.0` to `3.37.1`.
- Updated `Microsoft.EntityFrameworkCore` from version `8.0.0` to `8.0.1`.
- Updated `Microsoft.Extensions.Options` from version `8.0.0` to `8.0.1`.
- Updated `SharpToken` from version `1.2.12` to `1.2.14`.
- Updated `xunit` from version `2.6.2` to `2.6.6`.
- Updated `xunit.analyzers` from version `1.6.0` to `1.10.0`.
- Updated `xunit.extensibility.core` from version `2.6.2` to `2.6.6`.
- Updated `xunit.runner.visualstudio` from version `2.5.4` to `2.5.6`.
- Updated `StyleCop.Analyzers` from version `1.2.0-beta.507` to `1.2.0-beta.556`.
- Updated `System.Text.Json` from version `8.0.0` to `8.0.1`.
- Updated version from `8.0.3` to `8.1.0` due to all the major and breaking changes.
- Updated some `README.md` files changing `IKernel` for `Kernel`.
- Updated and added new unit tests to cover the main "happy path" of implementations that use `Semantic Kernel`.

### Minor Changes

- Replaced reference `Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer` (version `5.1.0`) for `Asp.Versioning.Mvc.ApiExplorer` (version `8.0.0`) which is the new name and implementation of the ASP.NET versioning libraries.
- Updated prompt function configuration files (`config.json`) to new format.
- Renamed files `IKernelExtensions` to `KernelExtensions.cs`.
- Fixed token counting in `ChatWithHistoryPlugin.cs`.
- Updated sample projects.
- Fixed some typos and grammatical errors.

## [8.0.3]

### Minor Changes
  - Add gpt-35-turbo-16k and gpt-3.5-turbo-16k model implementation allowed

## [8.0.2]

### Major Changes
 - In `Encamina.Enmarcha.SemanticKernel.Plugins.Text` Summarize Plugin, a new parameter `locale` has been added to control the output language of the generated summary. [(#34)](https://github.com/Encamina/enmarcha/issues/34)

## [8.0.1]

### Major Changes
 - In `Encamina.Enmarcha.SemanticKernel.Abstractions.ILengthFunctions`, `GptEncoding` is now cached and reused to improve performance. [(#30)](https://github.com/Encamina/enmarcha/pull/30)

### Minor Changes
  - Changes from version 8.0.0 have been added to the `CHANGELOG.md` file.

## [8.0.0]

### Major Changes

 - Changed .NET version from 6 to 8, therefore closes issue `Everything ready for ENMARCHA 8.0.0 #7`.
 - Updated the following .NET libraries to their newest version (8.0.0):
   - Microsoft.AspNetCore.Authentication.JwtBearer
   - Microsoft.AspNetCore.Authentication.OpenIdConnect
   - Microsoft.EntityFrameworkCore
   - Microsoft.EntityFrameworkCore.SqlServer
   - Microsoft.Extensions.Caching.Abstractions
   - Microsoft.Extensions.Configuration.Abstractions
   - Microsoft.Extensions.DependencyInjection.Abstractions
   - Microsoft.Extensions.Hosting
   - Microsoft.Extensions.Http
   - Microsoft.Extensions.Logging.Abstractions
   - Microsoft.Extensions.Options
   - Microsoft.Extensions.Options.ConfigurationExtensions
   - Microsoft.Extensions.Options.DataAnnotations
   - System.Net.Http.Json
   - System.Text.Json
 - Updated library Azure.Data.Tables from 12.8.1 to 12.8.2.
 - Updated library Microsoft.Azure.Cosmos from 3.36.0 to 3.37.0.
 - Updated Bot Framework related libraries from version 4.21.1 to 4.21.2. These libraries are:
   - Microsoft.Bot.Builder.Azure
   - Microsoft.Bot.Builder.Azure.Blobs
   - Microsoft.Bot.Builder.Dialogs
   - Microsoft.Bot.Builder.Integration.ApplicationInsights.Core
   - Microsoft.Bot.Builder.Integration.AspNet.Core
 - Updated library Moq from 4.20.69 to 4.20.70.
 - Updated library xunit from 2.6.1 to 2.6.2.
 - Updated library xunit.analyzers from 1.5.0 to 1.6.0.
 - Updated library xunit.extensibility.core from 2.6.1 to 2.6.2.
 - Updated library xunit.runner.visualstudio from 2.5.3 to 2.5.4.

## Minor Changes
 - Some minor tweaks.

## [6.0.4]

### Important

This version updates the `Semantic Kernel` library to version `1.0.0-beta8`, which introduces a lot of breaking changes in the code that mostly translate into multiple obsolescence warnings. Eventually, newer versions of this library will fix these warnings once a final version of `Semantic Kernel` is used.

The main motivation for this update is to take advantage of the latest improvements in the `Semantic Kernel` library, like the `Stepwise Planner` or Function Calls, plus better integrations with LLMs like OpenAI, among many other improvements.

Sadly, some warnings regarding types or members obsolescence could not be addresses until the Microsoft team behind `Semantic Kernel` provides a final version of the library. So far, these warnings are:

 - CS0618: *IKernel.PromptTemplateEngine' is obsolete: 'PromptTemplateEngine has been replaced with PromptTemplateFactory and will be null. If you pass an PromptTemplateEngine instance when creating a Kernel it will be wrapped in an instance of IPromptTemplateFactory. This will be removed in a future release.*
 - CS0618: *ISKFunction.RequestSettings' is obsolete: 'Use PromptTemplateConfig.ModelSettings instead. This will be removed in a future release.*
 - CS0618: *ISKFunction.SkillName' is obsolete: 'Methods, properties and classes which include Skill in the name have been renamed. Use ISKFunction.SkillName instead. This will be removed in a future release.*
 
### Breaking Changes

- Updated `Semantic Kernel` libraries to version `1.0.0-beta8`.
- `Encamina.Enmarcha.SemanticKernel.Abstractions`
  * Removed method `ValidateAndThrowIfErrorOccurred`.
  * Removed properties `ChatModelName`, `CompletionsModelName`, and `EmbeddingsModelName` from `SemanticKernelOptions`.
- The following methods do not throw an `ArgumentException` if the instance of `ISKFunction` is not a semantic function, since now `Semantic Kernel` does not longer differentiates between Semantic and Native functions:
  * GetSemanticFunctionPromptAsync
  * GetSemanticFunctionUsedTokensAsync
- The extension method `ImportQuestionAnsweringPlugin` in `Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering` does not import the Memory plugin anymore. If the usage of the Question Answering plugin requires memory support, use the `ImportQuestionAnsweringPluginWithMemory` extension method instead. Remember to add a valid instance of `ISemanticTextMemory` as a service in the dependency container.

### Major Changes

- New extension method `AddSemanticTextMemory` in `Encamina.Enmarcha.SemanticKernel.Connectors.Memory` to add a semantic text memory (i.e., `ISemanticTextMemory`) to the dependency container.
- New extension method `ImportQuestionAnsweringPluginWithMemory` in `Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering` to support memories when getting context for the Question Answering plugin. Remember to add a valid instance of `ISemanticTextMemory` as a service in the dependency container.
- Added `Directory.Build.targets` at Samples level to prevent generating NuGet packages of these projects.

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