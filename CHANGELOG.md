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

## [8.1.6]

### Breaking Changes 

 - Renamed `UserId` to `IndexerId` in `ChatMessageHistoryRecord`. This change requires consumers to update their database to match the new property name. 
    - In case of using Cosmos DB, `IndexerId` should be the new partition key of the collection. You can learn how to change the partition key and do the data migration [here](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/change-partition-key).

### Minor Changes

 - Added `SemanticKernelCosineStringSimilarityComparer` in `Encamina.Enmarcha.SemanticKernel` to compare two strings using cosine similarity algorithm.


## [8.1.5]

### Breaking Changes 

 - In `AzureOpenAIOptions` the default value of `ServiceVersion` changes from `V2023_12_01_Preview` to `V2024_02_15_Preview` since the former is **deprecated**.
 - In the `QuestionAnsweringFromMemoryQuery` function of the `QuestionAnsweringPlugin`, a `null` value is no longer returned when there are no relevant memory results. Instead, the execution flow continues, prompting a message with an empty context information, ultimately resulting in a response such as "I don't know" or a similar message.
 - Added a new method `GetDocumentContentAsync` to the `IDocumentContentExtractor` interface, which is now required to be implemented.

### Major Changes

- In interface type `IChatHistoryProvider` added new method `DeleteChatMessagesHistoryAsync` to delete a user's chat history messages. This method is implemented in `ChatHistoryProvider`.
- Added new interface `Encamina.Enmarcha.AI.Abstractions.ISemanticTextSplitter` and its implementations `Encamina.Enmarcha.AI.SemanticTextSplitter` to split a text into meaningful chunks based on embeddings.
- Added a new utility class for mathematical operations `Encamina.Enmarcha.Core.MathUtils`.
- Fixed `DeleteAsync<TEntityId>` method in `CosmosRepository<T>`. This method was always throwing exceptions because the partition key value was always `null`. It is fixed by considering the `Id` to delete the whole partition. If a specific item in the partition should be removed, then use the `DeleteAsync` on-generic method.
- Added `DefaultDocumentContentSemanticExtractor` to retrieve semantic chunks from documents.
- Bug fix in the `MathUtils.Quartiles` method.
- Enhanced `SplitAsync` in `Encamina.Enmarcha.AI.SemanticTextSplitter` to iteratively split chunks exceeding `options.MaxChunkSize` with a retry limit of `options.ChunkSplitRetryLimit`.
- Updated dependencies:
  - Updated `Bogus` from `35.4.0` to `35.4.1`.
  - Updated `Azure.Core` from `1.37.0` to `1.38.0`.
  - Updated `Azure.OpenAI` from `1.0.0-beta13` to `1.0.0-beta14`.
  - Updated `MailKit` from `4.3.0` to `4.4.0`.
  - Updated `MimeKit` from `4.3.0` to `4.4.0`.
  - Updated `Microsoft.Bot.Builder.Azure` from `4.22.1` to `4.22.2`.
  - Updated `Microsoft.Bot.Builder.Azure.Blobs` from `4.22.1` to `4.22.2`.
  - Updated `Microsoft.Bot.Builder.Dialogs` from `4.22.1` to `4.22.2`.
  - Updated `Microsoft.Bot.Builder.Integration.ApplicationInsights.Core` from `4.22.1` to `4.22.2`.
  - Updated `MMicrosoft.Bot.Builder.Integration.AspNet.Core` from `4.22.1` to `4.22.2`.
  - Updated `Microsoft.SemanticKernel.Abstractions` from `1.4.0` to `1.6.2`.
  - Updated `Microsoft.SemanticKernel.Connectors.AzureAISearch` from `1.4.0-alpha` to `1.6.2-alpha`. This does fix the [Issue 72](https://github.com/Encamina/enmarcha/issues/72).
  - Updated `Microsoft.SemanticKernel.Connectors.OpenAI` from `1.4.0` to `1.6.2`.
  - Updated `Microsoft.SemanticKernel.Connectors.Qdrant` from `1.4.0-alpha` to `1.6.2-alpha`.
  - Updated `Microsoft.SemanticKernel.Core` from `1.4.0` to `1.6.2`.
  - Updated `Microsoft.SemanticKernel.Plugins.Document` from `1.4.0-alpha` to `1.6.2-alpha`.
  - Updated `Microsoft.SemanticKernel.Plugins.Memory` from `1.4.0-alpha` to `1.6.2-alpha`.
  - Updated `SharpToken` from `1.2.15` to `1.2.17`.
  - Updated `coverlet.collector` from `6.0.0` to `6.0.1`.
  - Updated `xunit` from `2.6.6` to `2.7.0`.
  - Updated `xunit.analyzers` from `1.10.0` to `1.11.0`.
  - Updated `xunit.extensibility.core` from `2.6.6` to `2.7.0`.
  - Updated `xunit.runner.visualstudio` from `2.5.6` to `2.5.7`.
  
### Minor Changes

- Changes in the prompt of `QuestionAnsweringPlugin` to enhance language detection in the response.
- Slight improvement in the `DeleteAsync` method in `CosmosRepository`.
- Some boy scouting and typo fixes in comments.
- Added new extension method to add Qdrant and Azure Search AI as Keyed Memory Store.
- Added new function to delete chat messages history in `ChatWithHistoryPlugin`.
- Added new model `gpt-4-turbo` to `ModelInfo`.

### Important

Some features from `Semantic Kernel` that we might have been using, are marked as ***experimental*** and produce warnings that do not allow the compilation of the code. To use these features, these warnings must be ignored explicitly per project. The following is a list of these warnings and the affected projects:
- SKEXP0001:
    - `Encamina.Enmarcha.SemanticKernel.Abstractions`
    - `Encamina.Enmarcha.SemanticKernel.Connectors.Memory`
    - `Encamina.Enmarcha.SemanticKernel.Plugins.Memory`
    - `Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering`
    - `Encamina.Enmarcha.Samples.SemanticKernel.QuestionAnswering`
- SKEXP0010:
    - `Encamina.Enmarcha.SemanticKernel.Connectors.Memory`
    - `Encamina.Enmarcha.Samples.SemanticKernel.QuestionAnswering`
- SKEXP0020:
    - `Encamina.Enmarcha.SemanticKernel.Connectors.Memory`
- SKEXP0050:
    - `Encamina.Enmarcha.SemanticKernel.Connectors.Document`
    - `Encamina.Enmarcha.Samples.SemanticKernel.QuestionAnswering`

Some warnings have also been removed with the new `Semantic Kernel` updates.
- SKEXP0003:
    - `Encamina.Enmarcha.Samples.SemanticKernel.QuestionAnswering`
    - `Encamina.Enmarcha.SemanticKernel`
    - `Encamina.Enmarcha.SemanticKernel.Abstractions`
    - `Encamina.Enmarcha.SemanticKernel.Connectors.Memory`
    - `Encamina.Enmarcha.SemanticKernel.Plugins.Memory`
    - `Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering`
- SKEXP0011:
    - `Encamina.Enmarcha.Samples.SemanticKernel.QuestionAnswering`
    - `Encamina.Enmarcha.SemanticKernel.Connectors.Memory`
- SKEXP0021:
    - `Encamina.Enmarcha.SemanticKernel.Connectors.Memory`
- SKEXP0026:
    - `Encamina.Enmarcha.SemanticKernel.Connectors.Memory` 
- SKEXP0051:
    - `Encamina.Enmarcha.SemanticKernel.Connectors.Document`
- SKEXP0052:
    - `Encamina.Enmarcha.Samples.SemanticKernel.QuestionAnswering`


## [8.1.4]

### Major Changes
- Updated dependencies:
  - Updated `Microsoft.AspNetCore.Authentication.JwtBearer` from `8.0.1` to `8.0.2`.
  - Updated `Microsoft.AspNetCore.Authentication.OpenIdConnect` from `8.0.1` to `8.0.2`.
  - Updated `Microsoft.Azure.Cosmos` from `3.38.0` to `3.38.1`.
  - Updated `Microsoft.EntityFrameworkCore` from `8.0.1` to `8.0.2`.
  - Updated `Microsoft.EntityFrameworkCore.SqlServer` from `8.0.1` to `8.0.2`.
  - Updated `Microsoft.Extensions.Azure` from `1.7.1` to `1.7.2`.  
  - Updated `Microsoft.Extensions.Options` from `8.0.1` to `8.0.2`.
  - Updated `Microsoft.NET.Test.Sdk` from `17.8.0` to `17.9.0`.
  - Updated `Microsoft.SemanticKernel.Abstractions` from `1.3.1` to `1.4.0`.
  - Updated `Microsoft.SemanticKernel.Connectors.AzureAISearch` from `1.3.1-alpha` to `1.4.0-alpha`. This does fix the [Issue 72](https://github.com/Encamina/enmarcha/issues/72).
  - Updated `Microsoft.SemanticKernel.Connectors.OpenAI` from `1.3.1` to `1.4.0`.
  - Updated `Microsoft.SemanticKernel.Connectors.Qdrant` from `1.3.1-alpha` to `1.4.0-alpha`.
  - Updated `Microsoft.SemanticKernel.Core` from `1.3.1` to `1.4.0`.
  - Updated `Microsoft.SemanticKernel.Plugins.Document` from `1.3.1-alpha` to `1.4.0-alpha`.
  - Updated `Microsoft.SemanticKernel.Plugins.Memory` from `1.3.1-alpha` to `1.4.0-alpha`.
  - Updated `System.Text.Json` from `8.0.1` to `8.0.2`.
- Bug fix ([Issue 72](https://github.com/Encamina/enmarcha/issues/72)). Removed `MemoryStoreExtender` workaround after updating to `Microsoft.SemanticKernel.Connectors.AzureAISearch` version `1.4.0-alpha` which resolves the issue.

## [8.1.3]

### Breaking Changes  
- Removed `HistoryMaxMessages` property from `ChatWithHistoryPluginOptions` as part of a refactor to improve the configuration management of chat history. This property is now available within a new dedicated class `ChatHistoryProviderOptions`, which is designed to configure aspects of the `IChatHistoryProvider` implementation.
- The method `ImportChatWithHistoryPluginUsingCosmosDb` has been renamed to `ImportChatWithHistoryPlugin` to reflect its decoupling from the specific storage implementation and to align with the new `IChatHistoryProvider` abstraction. This change requires consumers to update their method calls to match the new signature, and to provide an instance of `IChatHistoryProvider` in the dependency container. You can use `AddCosmosChatHistoryProvider` to add an instance of `IChatHistoryProvider` that uses Azure Cosmos DB for storing chat histories.
- Modified the `ChatAsync` method signature in `ChatWithHistoryPlugin` by changing the order of parameters and making `userName` and `locale` optional. This change requires consumers to update their method calls to match the new signature. 
- The `KernelExtensions.cs` and `ChatWithHistoryPluginOptions.cs` files in `Encamina.Enmarcha.SemanticKernel.Plugins.Chat` had been moved to a new location to better align with the project's structure. 

### Major Changes
- Introduced `IChatHistoryProvider` interface and its corresponding implementation `ChatHistoryProvider`. This new abstraction layer provides a more flexible and decoupled way to work with chat history.
- Added a new extension method `AddCosmosChatHistoryProvider` to the service collection extensions. This method streamlines the setup and registration of `IChatHistoryProvider` that uses Azure Cosmos DB for storing chat histories.
- Removed direct dependency on `IAsyncRepository<ChatMessageHistoryRecord>` in `ChatWithHistoryPlugin`, now relying on `IChatHistoryProvider` for chat history management.  
- Added new calculation methods `LengthChatMessage` and `LengthChatMessageWithEncoding` in `ILengthFunctions` to determine the length of chat messages considering the author's role.  
- Updated dependencies:
  - Updated `Azure.AI.OpenAI` from `1.0.0-beta12` to `1.0.0-beta13` (which provides some fixes for Function Calling).
  - Updated `Azure.Data.Tables` from `12.8.2` to `12.8.3`.
  - Updated `Microsoft.Bot.Builder.Azure` from `4.22.0` to `4.22.1`.
  - Updated `Microsoft.Bot.Builder.Azure.Blobs` from `4.22.0` to `4.22.1`.
  - Updated `Microsoft.Bot.Builder.Dialogs` from `4.22.0` to `4.22.1`.
  - Updated `Microsoft.Bot.Builder.Integration.ApplicationInsights.Core` from `4.22.0` to `4.22.1`.
  - Updated `MMicrosoft.Bot.Builder.Integration.AspNet.Core` from `4.22.0` to `4.22.1`.
  - Updated `Microsoft.Semantic Kernel` from `1.3.0` to `1.3.1`.
  - Updated `Microsoft.SemanticKernel.Connectors.AzureAISearch` from `1.3.0-alpha` to `1.3.1-alpha`. **Important**: this does not fix the issue detected by ENMARCHA and reported in [Issue 72](https://github.com/Encamina/enmarcha/issues/72).
  - Updated `Microsoft.SemanticKernel.Connectors.Qdrant` from `1.3.0-alpha` to `1.3.1-alpha`.
  - Updated `Microsoft.SemanticKernel.Plugins.Document` from `1.3.0-alpha` to `1.3.1-alpha`.
  - Updated `Microsoft.SemanticKernel.Plugins.Memory` from `1.3.0-alpha` to `1.3.1-alpha`.
  - Updated `SharpToken` from `1.2.14` to `1.2.15`.

### Minor Changes
- Added `Description` property in `VersionSwaggerGenOptions`.
- New text prompt function for extract KeyPhrases with specified locale, `KeyPhrasesLocaled`.
- Added an example of using `KeyPhrasesLocaled` in `Encamina.Enmarcha.Samples.SemanticKernel.Text`.
- New text prompt function for translate texts, `Translate`.
- Added an example of using `Translate` in `Encamina.Enmarcha.Samples.SemanticKernel.Text`.
- Bug fix: Temporary workaround for handling Http NotFound exception in `MemoryStoreExtender`. [(#72)](https://github.com/Encamina/enmarcha/issues/72)
- Added new method `ExistsMemoryAsync` in `MemoryStoreExtender`.
- Added a new optional parameter `Locale` to the functions of `QuestionAnsweringPlugin`, to specify the language of the response.
- Improved memory store event types, when they are raised and the data inside the arguments.
- Added new package `Encamina.Enmarcha.AspNet.OpenApi` with some goodies for OpenAPI. Currently, it adds the following:
    - `GroupNameKeyAuthorizationMiddleware` a middleware that provides key authorization for OpenAPI specifications based on the group name of an API.
    - `GroupNameKeyAuthenticationOptions` an options class to configure the `GroupNameKeyAuthorizationMiddleware`.
    - Extensions method on `IApplicationBuilder` to add the `GroupNameKeyAuthorizationMiddleware`. For more information, refer to the package `README.md`.
- Adjusted package references in `Encamina.Enmarcha.SemanticKernel.csproj` to include `Encamina.Enmarcha.Data.Abstractions`.  

## [8.1.2]

### Important

This version updates the `Semantic Kernel` library from version `1.1.0` to `1.3.0`, which introduces minor changes in the code, mostly internal dependencies.

For more information about these changes, please visit the following links: 
 - [Semantic Kernel release dotnet-1.2.0](https://github.com/microsoft/semantic-kernel/releases/tag/dotnet-1.2.0)
 - [Semantic Kernel release dotnet-1.3.0](https://github.com/microsoft/semantic-kernel/releases/tag/dotnet-1.3.0)

### Breaking Changes
- Replace dependency with `IMemoryStore` for `IMemoryManager` in abstract class `MemoryStoreHandlerBase`. This affects internal types like the `EphemeralMemoryStoreHandler`.
- Removed visibility modifiers in `IMemoryManager` interface.
- Signature of `UpsertMemoryAsync` method has changed in `IMemoryManager` interface.
- Signature of `BatchUpsertMemoriesAsync` method has changed in `IMemoryManager` interface.
- Dependency with `Kernel` has been removed in `MemoryManager` class. Also, added dependency with `ILogger`.
- Added method overloads to pass `Encamina.Enmarcha.AI.Abstractions.TextSplitterOptions` when splitting text in `Encamina.Enmarcha.AI.Abstractions.ITextSplitter` and its implementations.

### Major change
- Method `GetDocumentConnector` in `DocumentContentExtractorBase` is now `public` instead of `protected`.
- New `MemoryManager` property of type `IMemoryManager` in `IMemoryStoreHandler` interface to get read-only access to the underlaying memory manager.
- New `MemoryStore` property of type `IMemoryStore` in `IMemoryManager` interface to get read-only access to the underlaying memory store.
- Removed unnecessary `Guards` when adding a Memory Manager and the Ephemeral Memory Store Handler. The exceptions will be thrown by the DI engine itself.
- Added new class `AzureAISearchOptions` to configure connection parameters for Azure AI Search.
- Added new extension method `AddAzureAISearchMemoryStore` to add Azure AI Search as a valid vector database for a `IMemoryStore` instance.
- Improved extensions methods for adding `IMemoryStore` to consider debouncing when monitoring changes in parameters.
- Updated dependencies:
  - Updated `Semantic Kernel` from `1.1.0` to `1.3.0` (third final version of `Semantic Kernel`).
  - Updated `Microsoft.Azure.Cosmos` from version `3.37.1` to `3.38.0`.
  - Updated `Microsoft.Bot.Builder.Azure` from version `4.21.2` to `4.22.0`.
  - Updated `Microsoft.Bot.Builder.Azure.Blobs` from version `4.21.2` to `4.22.0`.
  - Updated `Microsoft.Bot.Builder.Dialogs` from version `4.21.2` to `4.22.0`.
  - Updated `Microsoft.Bot.Builder.Integration.ApplicationInsights.Core` from version `4.21.2` to `4.22.0`.

### Minor Changes
- Properties `CollectionNamePostfix` and `CollectionNamePrefix` from `MemoryStoreHandlerBase` are now `virtual` instead of `abstract`.
- In `EphemeralMemoryStoreHandler`, property `CollectionNamePrefix` has the value `ephemeral-` fixed.
- Fixed some typos and grammatical errors (mostly on code comments).
- Added new extension method `AddDefaultDocumentConnectorProvider` in `Encamina.Enmarcha.SemanticKernel.Connectors.Document` to get access to a default implementation of a `IDocumentConnector`.
- Updated sample projects with latest changes.
- Overloaded `AddDefaultDocumentConnectorProvider` and `AddDefaultDocumentContentExtractor` methods with a parameter to pass a function to calculate the length of a text and inject it as a dependency.
- Added `README.md` files to all projects in the solution when publishing to NuGet.org.
- Added event handler for `IMemoryStore` operations.
- Added new extension method `GetKernelPromptAsync` in `Encamina.Enmarcha.SemanticKernel.Extensions.KernelExtensions` to retrieve the final prompt for a given prompt using the arguments.
- Added new extension method `GetKernelFunctionUsedTokensFromPromptAsync` in `Encamina.Enmarcha.SemanticKernel.Extensions.KernelExtensions` to obtain the total number of tokens used in generating a prompt from an inline prompt function.
- Fixed `GetMaxTokensFromKernelFunction` in `Encamina.Enmarcha.SemanticKernel.Extensions.KernelExtensions`. Now, it considers whether the arguments are of type `OpenAIPromptExecutionSettings` when obtaining the MaxTokens.
- New `IMemoryStoreExtender` type which obsolesces the `IMemoryManager` type and its references. If you are using `IMemoryManager` start a plan to replace it with `IMemoryStoreExtender`.
- Added default values for `TextSplitterOptions` to allow using it without explicit configuration.
- Some code improvements to use .NET 8 and C# 12 features.
- Added new `Debouncer` class to provide mechanisms to prevent multiple calls to a method or event.

## [8.1.1]

### Minor Changes
  - Fixed `BuildConfiguration` not set to Release in `.github/workflows/main.yml` for the main branch. [(#31)](https://github.com/Encamina/enmarcha/issues/31)

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

### Breaking Changes

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