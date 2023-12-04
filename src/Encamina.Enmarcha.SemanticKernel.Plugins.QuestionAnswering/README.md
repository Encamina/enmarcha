# Semantic Kernel - Question Answering Plugin

[![Nuget package](https://img.shields.io/nuget/v/Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering)](https://www.nuget.org/packages/Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering)

The Question Answering Plugin is a project that provides functionality as a plugin for answering questions based on a given context or based on the information stored in the Semantic Kernel's memory.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.SemanticKernel.QuestionAnswering.Plugins](https://www.nuget.org/packages/Encamina.Enmarcha.SemanticKernel.QuestionAnswering.Plugins) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.SemanticKernel.QuestionAnswering.Plugins

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.SemanticKernel.QuestionAnswering.Plugins](https://www.nuget.org/packages/Encamina.Enmarcha.SemanticKernel.QuestionAnswering.Plugins) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.SemanticKernel.QuestionAnswering.Plugins

## How to use

Within the `QuestionAnsweringPlugin`, there are two available functions for answering questions. On the one hand, there is a function in [QuestionAnsweringFromContext](./Plugins/QuestionAnsweringPlugin/QuestionAnsweringFromContext/skprompt.txt), which, given a context and a question, it answers the question based on the provided context. On the other hand, the plugin [QuestionAnsweringFromMemoryQuery](./Plugins/QuestionAnsweringPlugin.cs) searches within Semantic Kernel's memory for the most relevant results to the posed question, which are provided as the context to answer the question.

### QuestionAnsweringFromContext Function

This is a semantic function ([skprompt.txt](./Plugins/QuestionAnsweringPlugin/QuestionAnsweringFromContext/skprompt.txt)/[config.json](./Plugins/QuestionAnsweringPlugin/QuestionAnsweringFromContext/config.json)) within Semantic Kernel that provides an answer to a question based on a provided context using Language Model Models (LLMs) such as OpenAI, Azure OpenAI, to name but a few.

To use it, the first step is to import it into Semantic Kernel.
```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ... 

builder.Services.AddScoped(sp =>
{
    var kernel = new KernelBuilder()
        .WithAzureChatCompletionService("<YOUR DEPLOYMENT NAME>", "<YOUR AZURE ENDPOINT>", "<YOUR API KEY>", alsoAsTextCompletion: true)
        //.WithAzureTextCompletionService("<YOUR DEPLOYMENT NAME>", "<YOUR AZURE ENDPOINT>", "<YOUR API KEY>")
        /// ...
        .Build();

    // ...

    kernel.ImportQuestionAnsweringPlugin(sp, ILengthFunctions.LengthByTokenCount);

    return kernel;
});
```

Now you can inject the kernel via constructor, and the question capabilities are already available.

```csharp
public class MyClass
{
    private readonly IKernel kernel;

    public MyClass(IKernel kernel)
    {
        this.kernel = kernel;
    }

    public async Task TestQuestionAnsweringFromContextAsync()
    {
        var contextVariables = new ContextVariables();
        contextVariables.Set(PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromContext.Parameters.Input, "What year was the French Revolution?");
        contextVariables.Set(PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromContext.Parameters.Context, 
            @"The French Revolution[a] was a period of radical political and societal change in France that began with the Estates General of 1789, 
and ended with the formation of the French Consulate in November 1799. Many of its ideas are considered fundamental principles of liberal democracy,
while the values and institutions it created remain central to French political discourse. Its causes are generally agreed to be a combination of social,
political and economic factors, which the Ancien Régime proved unable to manage. In May 1789, widespread social distress led to the convocation of the Estates General,
which was converted into a National Assembly in June. Continuing unrest culminated in the Storming of the Bastille on 14 July, which led to a series of radical 
measures by the Assembly, including the abolition of feudalism, the imposition of state control over the Catholic Church in France, and extension of the right 
to vote.");

        var functionQuestionAnswering = kernel.Func(PluginsInfo.QuestionAnsweringPlugin.Name, PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromContext.Name);

        var resultContext = await kernel.RunAsync(contextVariables, functionQuestionAnswering);
    }
}
```
In the previous example, the question has been included within the `Input` parameter, and the context from which the answer is derived is in the `Context` parameter. Within `resultContext`, you will find texts that say something like _The French Revolution began in 1789_.

### QuestionAnsweringFromMemoryQuery Function

This is a native function of Semantic Kernel that, given a question, searches for the most relevant result within Semantic Kernel's memory and uses that context to call the semantic function `QuestionAnsweringFromContext` in order to generate a response.

To use it, the first step is to import it into Semantic Kernel.

```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ... 

builder.Services.AddScoped(sp =>
{
    var kernel = new KernelBuilder()
        .WithAzureChatCompletionService("<YOUR DEPLOYMENT NAME>",  "<YOUR AZURE ENDPOINT>", "<YOUR API KEY>", alsoAsTextCompletion: true)
        .WithAzureTextEmbeddingGenerationService("<YOUR DEPLOYMENT NAME>",  "<YOUR AZURE ENDPOINT>", "<YOUR API KEY>")
        //.WithAzureTextCompletionService("<YOUR DEPLOYMENT NAME>", "<YOUR AZURE ENDPOINT>", "<YOUR API KEY>")
        //.WithOpenAITextEmbeddingGenerationService("<YOUR MODEL ID>", "<YOUR API KEY>", "<YOUR API KEY>")
        /// ...
        .Build();

    // ...
    
    var questionAnsweringPlugin = new QuestionAnsweringPlugin(kernel, "<YOUR DEPLOYMENT NAME>", ILengthFunctions.LengthByTokenCount);

    kernel.ImportQuestionAnsweringPlugin(sp, ILengthFunctions.LengthByTokenCount);

    // Requires Encamina.Enmarcha.SemanticKernel.Plugins.Memory nuget
    kernel.ImportMemoryPlugin(ILengthFunctions.LengthByTokenCount);

    return kernel;
});
```

Now you can inject the kernel via constructor, and the memory question capabilities are already available.

```csharp
public class MyClass
{
    private readonly IKernel kernel;

    public MyClass(IKernel kernel)
    {
        this.kernel = kernel;
    }

    public async Task TestQuestionAnsweringFromMemoryAsync()
    {
        var contextVariables = new ContextVariables();
        contextVariables.Set(PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.Question, "What year was the French Revolution?");
        contextVariables.Set(PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.CollectionSeparator, ",");
        contextVariables.Set(PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.CollectionsStr, "collection-1,collection-2");
        contextVariables.Set(PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.MinRelevance, "0.8");
        contextVariables.Set(PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.ResultsLimit, "1");
        contextVariables.Set(PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.ResponseTokenLimit, "300");

        var functionQuestionAnswering = kernel.Func(PluginsInfo.QuestionAnsweringPlugin.Name, PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Name);

        var resultContext = await kernel.RunAsync(contextVariables, functionQuestionAnswering);
    }
}
```