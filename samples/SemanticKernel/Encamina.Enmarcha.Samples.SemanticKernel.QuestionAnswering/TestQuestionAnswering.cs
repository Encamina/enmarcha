using Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering;

using Microsoft.SemanticKernel;

namespace Encamina.Enmarcha.Samples.SemanticKernel.QuestionAnswering;

/// <summary>
/// Example class of Question Answering Plugin.
/// </summary>
internal class TestQuestionAnswering
{
    private readonly Kernel kernel;

    public TestQuestionAnswering(Kernel kernel)
    {
        this.kernel = kernel;
    }

    public async Task<string> TestQuestionAnsweringFromContextAsync()
    {
        var input = @"What year was the French Revolution?";
        var context = @"The French Revolution[a] was a period of radical political and societal change in France that began with the Estates General of 1789, 
and ended with the formation of the French Consulate in November 1799. Many of its ideas are considered fundamental principles of liberal democracy,
while the values and institutions it created remain central to French political discourse. Its causes are generally agreed to be a combination of social,
political and economic factors, which the Ancien Régime proved unable to manage. In May 1789, widespread social distress led to the convocation of the Estates General,
which was converted into a National Assembly in June. Continuing unrest culminated in the Storming of the Bastille on 14 July, which led to a series of radical 
measures by the Assembly, including the abolition of feudalism, the imposition of state control over the Catholic Church in France, and extension of the right 
to vote.";

        Console.WriteLine($"# Context: {context} \n");
        Console.WriteLine($"# Question: {input} \n");

        var arguments = new KernelArguments()
        {
            [PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromContext.Parameters.Input] = input,
            [PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromContext.Parameters.Context] = context,
        };

        var functionQuestionAnswering = kernel.Plugins.GetFunction(PluginsInfo.QuestionAnsweringPlugin.Name, PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromContext.Name);
        var result = await kernel.InvokeAsync<string>(functionQuestionAnswering, arguments);

        Console.WriteLine($"# Result: {result} \n");

        return result;
    }

    public async Task<string> TestQuestionAnsweringFromMemoryAsync()
    {
        var input = "What period occurred the Industrial Revolution?";
        var arguments = new KernelArguments()
        {
            [PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.Question] = input,
            [PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.CollectionSeparator] = ",",
            [PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.CollectionsStr] = "my-collection",
            [PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.MinRelevance] = 0.8,
            [PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.ResultsLimit] = 1,
            [PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.ResponseTokenLimit] = 300,
            [PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.Locale] = "Italian",
        };

        Console.WriteLine($"# Question: {input} \n");

        var functionQuestionAnswering = kernel.Plugins.GetFunction(PluginsInfo.QuestionAnsweringPlugin.Name, PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Name);
        var result = await kernel.InvokeAsync<string>(functionQuestionAnswering, arguments);

        Console.WriteLine($"# Result: {result} \n");

        return result;
    }
}
