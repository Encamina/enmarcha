using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;

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
            var options = hostContext.Configuration.GetRequiredSection(nameof(SemanticKernelOptions)).Get<SemanticKernelOptions>()
                ?? throw new InvalidOperationException(@$"Missing configuration for {nameof(SemanticKernelOptions)}");

            // Add Semantic Kernel options
            services.AddOptions<SemanticKernelOptions>().Bind(hostContext.Configuration.GetSection(nameof(SemanticKernelOptions))).ValidateDataAnnotations().ValidateOnStart();

            // Here use the desired implementation (Qdrant, Volatile...)
            services.AddSingleton<IMemoryStore, VolatileMemoryStore>();

            // Initialize semantic memory for text (i.e., ISemanticTextMemory).
            services.AddSingleton(sp =>
            {
                return new MemoryBuilder()
                    .WithAzureOpenAITextEmbeddingGenerationService(options.EmbeddingsModelDeploymentName, options.Endpoint.ToString(), options.Key)
                    .WithMemoryStore(new VolatileMemoryStore())
                    .Build();
            });

            services.AddScoped(sp =>
            {
                // Initialize semantic kernel
                var kernel = new KernelBuilder()
                    .WithAzureOpenAIChatCompletionService(options.ChatModelDeploymentName, options.Endpoint.ToString(), options.Key)
                    .WithAzureOpenAITextEmbeddingGenerationService(options.EmbeddingsModelDeploymentName, options.Endpoint.ToString(), options.Key)
                    .Build();

                // Import Question Answering plugin
                kernel.ImportQuestionAnsweringPlugin(sp, ILengthFunctions.LengthByTokenCount);

                return kernel;
            });

            services.AddMemoryManager();
        });

        var host = hostBuilder.Build();

        // Initialize mock memory
        var mockMemoryInformation = new MockMemoryInformation(host.Services.GetService<IMemoryManager>(), host.Services.GetService<IMemoryStore>());
        await mockMemoryInformation.CreateCollection();
        await mockMemoryInformation.SaveDataMockAsync();

        // Initialize Q&A from Memory
        var testQuestionAnswering = new TestQuestionAnswering(host.Services.GetService<IKernel>());
        var result = await testQuestionAnswering.TestQuestionAnsweringFromMemoryAsync();

        Console.WriteLine($@"RESULT: {result}");
    }
}
