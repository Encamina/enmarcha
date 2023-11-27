using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Plugins.Memory;
using Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

using Sample_SemanticKernelQuestionAnswering.Memory;
using Sample_SemanticKernelQuestionAnswering.QuestionAnsweringPlugin;

namespace Sample_SemanticKernelQuestionAnswering;

public static class Example_QuestionAnsweringFromMemory
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
            // Add Semantic Kernel options
            services.AddOptions<SemanticKernelOptions>().Bind(hostContext.Configuration.GetSection(nameof(SemanticKernelOptions))).ValidateDataAnnotations().ValidateOnStart();

            // Here use the desired implementation (Qdrant, Volatile...)
            services.AddSingleton<IMemoryStore, VolatileMemoryStore>();

            services.AddScoped(sp =>
            {
                // Get semantic kernel options
                var options = hostContext.Configuration.GetRequiredSection(nameof(SemanticKernelOptions)).Get<SemanticKernelOptions>()
                ?? throw new InvalidOperationException(@$"Missing configuration for {nameof(SemanticKernelOptions)}");

                // Initialize semantic kernel
                var kernel = new KernelBuilder()
                    .WithMemoryStorage(sp.GetService<IMemoryStore>())
                    .WithAzureChatCompletionService(options.ChatModelDeploymentName, options.Endpoint.ToString(), options.Key)
                    .WithAzureTextEmbeddingGenerationService(options.EmbeddingsModelDeploymentName, options.Endpoint.ToString(), options.Key)
                    .Build();

                // Import Question Answering plugin
                kernel.ImportQuestionAnsweringPlugin(sp, ILengthFunctions.LengthByTokenCount);

                // Import Memory Plugin
                kernel.ImportMemoryPlugin(ILengthFunctions.LengthByTokenCount);

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
    }
}
