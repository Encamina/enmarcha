using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.AI.OpenAI.Azure;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;

using static System.Runtime.InteropServices.JavaScript.JSType;

using ILengthFunctions = Encamina.Enmarcha.SemanticKernel.Abstractions.ILengthFunctions;

namespace Encamina.Enmarcha.Samples.SemanticKernel.DocumentContentExtractor;

internal static class Program
{
    private static void Main(string[] _)
    {
        Console.Clear();

        // Create and configure builder
        var hostBuilder = new HostBuilder()
            .ConfigureAppConfiguration((configuration) =>
            {
                configuration.AddJsonFile(path: @"appsettings.json", optional: false, reloadOnChange: true)
                             .AddJsonFile(path: $@"appsettings.{Environment.UserName}.json", optional: true, reloadOnChange: true);
            });

        // Configure services
        hostBuilder.ConfigureServices((hostContext, services) =>
        {
            services.AddScoped(_ =>
            {
                // Get semantic kernel options
                var options = hostContext.Configuration
                    .GetRequiredSection(nameof(AzureOpenAIOptions))
                    .Get<AzureOpenAIOptions>() ?? throw new InvalidOperationException("Missing configuration for AzureOpenAIOptions");
                // Initialize semantic kernel
                var kernel = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(options.ChatModelDeploymentName, options.Endpoint.ToString(), options.Key)
                    .Build();

                return kernel;
            });

            services.AddRecursiveCharacterTextSplitter() // TODO: Should be commented...
                    .AddEnrichedRecursiveCharacterTextSplitter();

            //Correct registration of ILengthFunctions
            services.AddSingleton<Func<string, int>>(ILengthFunctions.LengthByCharacterCount);

            services.AddDocumentConnectors(hostContext.Configuration)
                    .AddDefaultDocumentConnectorProvider()
                    .AddDefaultDocumentContentExtractor() // TODO: Should be commented...
                    .AddDefaultDocumentContentEnrichedExtractor();

            services.AddHttpClient();
        });

        var host = hostBuilder.Build();

        // Initialize Examples
        var example = new Example(
            host.Services.GetRequiredService<Kernel>(),
            host.Services.GetRequiredService<IDocumentConnectorProvider>(),
            host.Services.GetRequiredService<IDocumentContentExtractor>(),
            host.Services.GetRequiredService<IDocumentContentEnrichedExtractor>());

        //example.ExtractDocumentContent();

        example.ExtractDocumentContentEnriched();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    public static IServiceCollection AddDocumentConnectors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptionsWithValidateOnStart<MistralAIDocumentConnectorOptions>()
                .Bind(configuration.GetSection(nameof(MistralAIDocumentConnectorOptions)))
                .ValidateDataAnnotations();

        services.AddWordDocumentConnector(configuration); // .docx
        services.AddParagraphPptxDocumentConnector();     // .pptx
        services.AddTxtDocumentConnector();               // .txt; .md
        services.AddSkVisionImageDocumentConnector(configuration); // .jpg; .jpeg; .png;

        // You can toggle this based on your needs
        //services.AddSingleton<IEnmarchaDocumentConnector, SkVisionStrictFormatCleanPdfDocumentConnector>(); // .pdf
        services.AddSingleton<IEnmarchaDocumentConnector, MistralAIDocumentConnector>(); // .pdf

        return services;
    }
}