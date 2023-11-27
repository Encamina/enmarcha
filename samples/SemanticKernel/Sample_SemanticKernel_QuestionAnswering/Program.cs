namespace Sample_SemanticKernelQuestionAnswering;
internal sealed class Program
{
    private static async Task Main(string[] args)
    {
        await Example_QuestionAnsweringFromContext.RunAsync();
        await Example_QuestionAnsweringFromMemory.RunAsync();
    }
}
