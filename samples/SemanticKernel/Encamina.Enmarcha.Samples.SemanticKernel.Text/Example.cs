using Encamina.Enmarcha.SemanticKernel.Plugins.Text;

using Microsoft.SemanticKernel;

namespace Encamina.Enmarcha.Samples.SemanticKernel.Text;

internal class Example
{
    private readonly Kernel kernel;

    private readonly string input = @"Alexandre Dumas born Dumas Davy de la Pailleterie, 24 July 1802 – 5 December 1870), also known as Alexandre Dumas père, was a French novelist and playwright.
        His works have been translated into many languages and he is one of the most widely read French authors. Many of his historical novels of adventure were originally published as serials, including The Count of Monte Cristo, The Three Musketeers, Twenty Years After and The Vicomte of Bragelonne: Ten Years Later.Since the early 20th century, his novels have been adapted into nearly 200 films.Prolific in several genres, Dumas began his career by writing plays, which were successfully produced from the first. He wrote numerous magazine articles and travel books; his published works totalled 100,000 pages.In the 1840s, Dumas founded the Théâtre Historique in Paris.
        His father, General Thomas - Alexandre Dumas Davy de la Pailleterie, was born in the French colony of Saint - Domingue(present - day Haiti) to Alexandre Antoine Davy de la Pailleterie, a French nobleman, and Marie-Cessette Dumas, an African slave.At age 14, Thomas - Alexandre was taken by his father to France, where he was educated in a military academy and entered the military for what became an illustrious career.
        Alexandre acquired work with Louis - Philippe, Duke of Orléans, then as a writer, a career which led to early success.Decades later, after the election of Louis - Napoléon Bonaparte in 1851, Dumas fell from favour and left France for Belgium, where he stayed for several years. He moved to Russia for a few years and then to Italy.In 1861, he founded and published the newspaper L'Indépendent, which supported Italian unification. He returned to Paris in 1864.
        English playwright Watts Phillips, who knew Dumas in his later life, described him as ""the most generous, large - hearted being in the world.He also was the most delightfully amusing and egotistical creature on the face of the earth.His tongue was like a windmill – once set in motion, you never knew when he would stop, especially if the theme was himself.""";

    public Example(Kernel kernel)
    {
        this.kernel = kernel;
        Console.WriteLine($"# Context: {input} \n");
    }

    public async Task TestSummaryAsync()
    {
        var summaryArguments = new KernelArguments()
        {
            [PluginsInfo.TextPlugin.Functions.Summarize.Parameters.Input] = input,
            [PluginsInfo.TextPlugin.Functions.Summarize.Parameters.MaxWordsCount] = 15,
        };

        var functionSummarize = kernel.Plugins.GetFunction(PluginsInfo.TextPlugin.Name, PluginsInfo.TextPlugin.Functions.Summarize.Name);

        var result = await kernel.InvokeAsync<string>(functionSummarize, summaryArguments);

        Console.WriteLine($"# Summary: {result} \n");
    }

    public async Task TextKeyPhrasesAsync()
    {
        var keyPhrasesArguments = new KernelArguments()
        {
            [PluginsInfo.TextPlugin.Functions.KeyPhrases.Parameters.Input] = input,
            [PluginsInfo.TextPlugin.Functions.KeyPhrases.Parameters.TopKeyphrases] = 2,
        };

        var functionKeyPhrases = kernel.Plugins.GetFunction(PluginsInfo.TextPlugin.Name, PluginsInfo.TextPlugin.Functions.KeyPhrases.Name);

        var result = await kernel.InvokeAsync<string>(functionKeyPhrases, keyPhrasesArguments);

        Console.WriteLine($"# Key Phrases: {result} \n");
    }

    public async Task TextKeyPhrasesLocaledAsync()
    {
        var keyPhrasesArguments = new KernelArguments()
        {
            [PluginsInfo.TextPlugin.Functions.KeyPhrasesLocaled.Parameters.Input] = input,
            [PluginsInfo.TextPlugin.Functions.KeyPhrasesLocaled.Parameters.Locale] = "Italian",
            [PluginsInfo.TextPlugin.Functions.KeyPhrasesLocaled.Parameters.TopKeyphrases] = 3,
        };

        var functionKeyPhrasesLocaled = kernel.Plugins.GetFunction(PluginsInfo.TextPlugin.Name, PluginsInfo.TextPlugin.Functions.KeyPhrasesLocaled.Name);

        var result = await kernel.InvokeAsync<string>(functionKeyPhrasesLocaled, keyPhrasesArguments);

        Console.WriteLine($"# Key Phrases Localed: {result} \n");
    }

    public async Task TextTranslateAsync()
    {
        var translateArguments = new KernelArguments()
        {
            [PluginsInfo.TextPlugin.Functions.Translate.Parameters.Input] = input,
            [PluginsInfo.TextPlugin.Functions.Translate.Parameters.Locale] = "Spanish",
        };

        var functionTranslate = kernel.Plugins.GetFunction(PluginsInfo.TextPlugin.Name, PluginsInfo.TextPlugin.Functions.Translate.Name);

        var result = await kernel.InvokeAsync<string>(functionTranslate, translateArguments);

        Console.WriteLine($"# Translation: {result} \n");
    }
}
