﻿namespace Encamina.Enmarcha.Samples.SemanticKernel.QuestionAnswering;

internal static class Program
{
    private static async Task Main(string[] _)
    {
        await ExampleQuestionAnsweringFromContext.RunAsync();
        await ExampleQuestionAnsweringFromMemory.RunAsync();
    }
}
