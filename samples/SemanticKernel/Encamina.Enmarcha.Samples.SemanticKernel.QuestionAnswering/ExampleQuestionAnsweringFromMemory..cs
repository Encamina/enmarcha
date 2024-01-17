using Encamina.Enmarcha.AI.OpenAI.Azure;
using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace Encamina.Enmarcha.Samples.SemanticKernel.QuestionAnswering;

internal static class ExampleQuestionAnsweringFromMemory
{
    public static async Task RunAsync()
    {
        Console.WriteLine("# Executing Example_QuestionAnsweringFromMemory \n");

        // Create and configure builder
        var hostBuilder = new HostBuilder()
            .ConfigureAppConfiguration((configuration) =>
            {
                configuration.AddJsonFile(path: @"appsettings.json", optional: false, reloadOnChange: true);
            });

        // Configure service
        hostBuilder.ConfigureServices((hostContext, services) =>
        {
            // Get semantic kernel options
            var options = hostContext.Configuration.GetRequiredSection(nameof(AzureOpenAIOptions)).Get<AzureOpenAIOptions>()
                ?? throw new InvalidOperationException(@$"Missing configuration for {nameof(AzureOpenAIOptions)}");

            // Add Semantic Kernel options
            services.AddOptions<AzureOpenAIOptions>().Bind(hostContext.Configuration.GetSection(nameof(AzureOpenAIOptions))).ValidateDataAnnotations().ValidateOnStart();

            // Here use any desired implementation (Qdrant, Volatile...)
            services.AddSingleton<IMemoryStore, VolatileMemoryStore>()
                .AddSemanticTextMemory();

            services.AddScoped(sp =>
            {
                // Initialize semantic kernel
                var kernel = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(options.ChatModelDeploymentName, options.Endpoint.ToString(), options.Key)
                    .AddAzureOpenAITextEmbeddingGeneration(options.EmbeddingsModelDeploymentName, options.Endpoint.ToString(), options.Key)
                    .Build();

                // Import Question Answering plugin
                kernel.ImportQuestionAnsweringPluginWithMemory(options, sp.GetRequiredService<ISemanticTextMemory>(), ILengthFunctions.LengthByTokenCount);

                return kernel;
            });

            services.AddMemoryManager();
        });

        var host = hostBuilder.Build();

        // Initialize mock memory
        var mockMemoryInformation = new MockMemoryInformation(host.Services.GetRequiredService<IMemoryManager>(), host.Services.GetRequiredService<IMemoryStore>());
        await mockMemoryInformation.CreateCollection();
        await mockMemoryInformation.SaveDataMockAsync();

        // Initialize Q&A from Memory
        var testQuestionAnswering = new TestQuestionAnswering(host.Services.GetService<Kernel>());
        var result = await testQuestionAnswering.TestQuestionAnsweringFromMemoryAsync();

        Console.WriteLine($@"RESULT: {result}");
    }
}
