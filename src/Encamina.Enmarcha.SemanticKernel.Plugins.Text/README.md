# Semantic Kernel - Text Plugin

The Text Plugin is a project that provides functionality to obtain various types of information from a text, such as a summary or keywords.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.SemanticKernel.Plugins.Text](https://www.nuget.org/packages/Encamina.Enmarcha.SemanticKernel.Plugins.Text) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.SemanticKernel.Plugins.Text

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.SemanticKernel.Plugins.Text](https://www.nuget.org/packages/Encamina.Enmarcha.SemanticKernel.Plugins.Text) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.SemanticKernel.Plugins.Text

## How to use

Within the TextPlugin, there are two available functions for text processing. On one hand, the function in [Summarize](./Plugins/TextPlugin/Summarize/skprompt.txt) provides the summary of a text. On the other hand, there is the function in [KeyPhrases](./Plugins/TextPlugin/KeyPhrases/skprompt.txt), which extracts keyphrases from a text.

To use TextPlugin, the first step is to import it into Semantic Kernel.

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

    kernel.ImportTextPlugin();

    return kernel;
});
```

Now you can inject the kernel via constructor, and the text capabilities are already available.

```csharp
public class MyClass
{
    private readonly IKernel kernel;

    public MyClass(IKernel kernel)
    {
        this.kernel = kernel;
    }

    public async Task TestSummaryAsync()
    {
        var contextVariables = new ContextVariables();
        contextVariables.Set(PluginsInfo.TextPlugin.Functions.Summarize.Parameters.Input, @"Alexandre Dumas born Dumas Davy de la Pailleterie, 24 July 1802 – 5 December 1870), also known as Alexandre Dumas père, was a French novelist and playwright.
        His works have been translated into many languages and he is one of the most widely read French authors. Many of his historical novels of adventure were originally published as serials, including The Count of Monte Cristo, The Three Musketeers, Twenty Years After and The Vicomte of Bragelonne: Ten Years Later. Since the early 20th century, his novels have been adapted into nearly 200 films. Prolific in several genres, Dumas began his career by writing plays, which were successfully produced from the first. He wrote numerous magazine articles and travel books; his published works totalled 100,000 pages. In the 1840s, Dumas founded the Théâtre Historique in Paris.
        His father, General Thomas-Alexandre Dumas Davy de la Pailleterie, was born in the French colony of Saint-Domingue (present-day Haiti) to Alexandre Antoine Davy de la Pailleterie, a French nobleman, and Marie-Cessette Dumas, an African slave. At age 14, Thomas-Alexandre was taken by his father to France, where he was educated in a military academy and entered the military for what became an illustrious career.
        Alexandre acquired work with Louis-Philippe, Duke of Orléans, then as a writer, a career which led to early success. Decades later, after the election of Louis-Napoléon Bonaparte in 1851, Dumas fell from favour and left France for Belgium, where he stayed for several years. He moved to Russia for a few years and then to Italy. In 1861, he founded and published the newspaper L'Indépendent, which supported Italian unification. He returned to Paris in 1864.
        English playwright Watts Phillips, who knew Dumas in his later life, described him as ""the most generous, large-hearted being in the world. He also was the most delightfully amusing and egotistical creature on the face of the earth. His tongue was like a windmill – once set in motion, you never knew when he would stop, especially if the theme was himself.""");
        contextVariables.Set(PluginsInfo.TextPlugin.Functions.Summarize.Parameters.MaxWordsCount, "15");

        var functionSummarize = kernel.Func(PluginsInfo.TextPlugin.Name, PluginsInfo.TextPlugin.Functions.Summarize.Name);

        var resultContext = await kernel.RunAsync(contextVariables, functionSummarize);
    }

    public async Task TextKeyPhrasesAsync()
    {
        var contextVariables = new ContextVariables();
        contextVariables.Set(PluginsInfo.TextPlugin.Functions.KeyPhrases.Parameters.Input, @"Alexandre Dumas born Dumas Davy de la Pailleterie, 24 July 1802 – 5 December 1870), also known as Alexandre Dumas père, was a French novelist and playwright.
        His works have been translated into many languages and he is one of the most widely read French authors. Many of his historical novels of adventure were originally published as serials, including The Count of Monte Cristo, The Three Musketeers, Twenty Years After and The Vicomte of Bragelonne: Ten Years Later. Since the early 20th century, his novels have been adapted into nearly 200 films. Prolific in several genres, Dumas began his career by writing plays, which were successfully produced from the first. He wrote numerous magazine articles and travel books; his published works totalled 100,000 pages. In the 1840s, Dumas founded the Théâtre Historique in Paris.
        His father, General Thomas-Alexandre Dumas Davy de la Pailleterie, was born in the French colony of Saint-Domingue (present-day Haiti) to Alexandre Antoine Davy de la Pailleterie, a French nobleman, and Marie-Cessette Dumas, an African slave. At age 14, Thomas-Alexandre was taken by his father to France, where he was educated in a military academy and entered the military for what became an illustrious career.
        Alexandre acquired work with Louis-Philippe, Duke of Orléans, then as a writer, a career which led to early success. Decades later, after the election of Louis-Napoléon Bonaparte in 1851, Dumas fell from favour and left France for Belgium, where he stayed for several years. He moved to Russia for a few years and then to Italy. In 1861, he founded and published the newspaper L'Indépendent, which supported Italian unification. He returned to Paris in 1864.
        English playwright Watts Phillips, who knew Dumas in his later life, described him as ""the most generous, large-hearted being in the world. He also was the most delightfully amusing and egotistical creature on the face of the earth. His tongue was like a windmill – once set in motion, you never knew when he would stop, especially if the theme was himself.""");
        contextVariables.Set(PluginsInfo.TextPlugin.Functions.KeyPhrases.Parameters.TopKeyphrases, "2");

        var functionSummarize = kernel.Func(PluginsInfo.TextPlugin.Name, PluginsInfo.TextPlugin.Functions.KeyPhrases.Name);

        var resultContext = await kernel.RunAsync(contextVariables, functionSummarize);
    }
}
```

In the first method, `TestSummaryAsync`, a summary of the text with a maximum of 15 words is being requested. Within `resultContext`, you will find texts that say something like _Alexandre Dumas, a French novelist and playwright, wrote famous adventure novels and founded a theater in Paris._.

In the second method, `TextKeyPhrasesAsync`, 2 keyphrases from the text are being requested. Within `resultContext`, you will find texts that say something like _Alexandre Dumas père_ and _French novelist and playwright_.