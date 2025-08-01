﻿using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.AI.OpenAI.Azure;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.SemanticKernel;

namespace Encamina.Enmarcha.Samples.SemanticKernel.DocumentContentExtractor;

internal static class Program
{
    private static void Main(string[] _)
    {
        // Create and configure builder
        var hostBuilder = new HostBuilder()
            .ConfigureAppConfiguration((configuration) =>
            {
                configuration.AddJsonFile(path: @"appsettings.json", optional: false, reloadOnChange: true)
                             .AddJsonFile($@"appsettings.{Environment.UserName}.json", optional: true, reloadOnChange: true);
            });

        // Configure service
        hostBuilder.ConfigureServices((hostContext, services) =>
        {
            services.AddScoped(_ =>
            {
                // Get semantic kernel options
                var options = hostContext.Configuration.GetRequiredSection(nameof(AzureOpenAIOptions)).Get<AzureOpenAIOptions>()
                ?? throw new InvalidOperationException(@$"Missing configuration for {nameof(AzureOpenAIOptions)}");

                // Initialize semantic kernel
                var kernel = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(options.ChatModelDeploymentName, options.Endpoint.ToString(), options.Key)
                    .Build();

                return kernel;
            });

            services.AddRecursiveCharacterTextSplitter()
            .AddSingleton(Enmarcha.SemanticKernel.Abstractions.ILengthFunctions.LengthByTokenCount);

            services.AddDocumentConnectors(hostContext.Configuration)
                .AddDefaultDocumentConnectorProvider()
                .AddDefaultDocumentContentExtractor()
                ;
        });

        var host = hostBuilder.Build();

        // Initialize Examples
        var example = new Example(host.Services.GetRequiredService<Kernel>(), 
            host.Services.GetRequiredService<IDocumentConnectorProvider>(), 
            host.Services.GetRequiredService<IDocumentContentExtractor>());

        example.ExtractDocumentContent();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    public static IServiceCollection AddDocumentConnectors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddWordDocumentConnector(); // .docx
        services.AddParagraphPptxDocumentConnector(); // .pptx
        services.AddTxtDocumentConnector(); // .txt; .md
        services.AddSkVisionImageDocumentConnector(configuration); // .jpg; .jpeg; .png;
        services.AddSingleton<IEnmarchaDocumentConnector, SkVisionStrictFormatCleanPdfDocumentConnector>(); // .pdf

        return services;
    }
}
