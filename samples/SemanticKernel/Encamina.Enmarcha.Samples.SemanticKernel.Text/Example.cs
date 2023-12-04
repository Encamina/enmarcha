using Encamina.Enmarcha.SemanticKernel.Plugins.Text;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace Encamina.Enmarcha.Samples.SemanticKernel.Text;

internal class Example
{
    private readonly IKernel kernel;

    private readonly string input = @"Alexandre Dumas born Dumas Davy de la Pailleterie, 24 July 1802 – 5 December 1870), also known as Alexandre Dumas père, was a French novelist and playwright.
        His works have been translated into many languages and he is one of the most widely read French authors. Many of his historical novels of adventure were originally published as serials, including The Count of Monte Cristo, The Three Musketeers, Twenty Years After and The Vicomte of Bragelonne: Ten Years Later.Since the early 20th century, his novels have been adapted into nearly 200 films.Prolific in several genres, Dumas began his career by writing plays, which were successfully produced from the first. He wrote numerous magazine articles and travel books; his published works totalled 100,000 pages.In the 1840s, Dumas founded the Théâtre Historique in Paris.
        His father, General Thomas - Alexandre Dumas Davy de la Pailleterie, was born in the French colony of Saint - Domingue(present - day Haiti) to Alexandre Antoine Davy de la Pailleterie, a French nobleman, and Marie-Cessette Dumas, an African slave.At age 14, Thomas - Alexandre was taken by his father to France, where he was educated in a military academy and entered the military for what became an illustrious career.
        Alexandre acquired work with Louis - Philippe, Duke of Orléans, then as a writer, a career which led to early success.Decades later, after the election of Louis - Napoléon Bonaparte in 1851, Dumas fell from favour and left France for Belgium, where he stayed for several years. He moved to Russia for a few years and then to Italy.In 1861, he founded and published the newspaper L'Indépendent, which supported Italian unification. He returned to Paris in 1864.
        English playwright Watts Phillips, who knew Dumas in his later life, described him as ""the most generous, large - hearted being in the world.He also was the most delightfully amusing and egotistical creature on the face of the earth.His tongue was like a windmill – once set in motion, you never knew when he would stop, especially if the theme was himself.""";

    /// <inheritdoc/>
    public Example(IKernel kernel)
    {
        this.kernel = kernel;
        Console.WriteLine($"# Context: {input} \n");
    }

    /// <inheritdoc/>
    public async Task TestSummaryAsync()
    {
        var contextVariables = new ContextVariables();
        contextVariables.Set(PluginsInfo.TextPlugin.Functions.Summarize.Parameters.Input, input);
        contextVariables.Set(PluginsInfo.TextPlugin.Functions.Summarize.Parameters.MaxWordsCount, "15");

        var functionSummarize = kernel.Functions.GetFunction(PluginsInfo.TextPlugin.Name, PluginsInfo.TextPlugin.Functions.Summarize.Name);

        var resultContext = await kernel.RunAsync(contextVariables, functionSummarize);
        var result = resultContext.GetValue<string>();

        Console.WriteLine($"# Summary: {result} \n");
    }

    /// <inheritdoc/>
    public async Task TextKeyPhrasesAsync()
    {
        var contextVariables = new ContextVariables();
        contextVariables.Set(PluginsInfo.TextPlugin.Functions.KeyPhrases.Parameters.Input, input);
        contextVariables.Set(PluginsInfo.TextPlugin.Functions.KeyPhrases.Parameters.TopKeyphrases, "2");

        var functionSummarize = kernel.Functions.GetFunction(PluginsInfo.TextPlugin.Name, PluginsInfo.TextPlugin.Functions.KeyPhrases.Name);

        var resultContext = await kernel.RunAsync(contextVariables, functionSummarize);
        var result = resultContext.GetValue<string>();

        Console.WriteLine($"# Key Phrases: {result} \n");
    }
}
