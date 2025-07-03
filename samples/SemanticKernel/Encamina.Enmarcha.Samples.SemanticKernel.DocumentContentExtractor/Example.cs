using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document;

using Microsoft.SemanticKernel;

namespace Encamina.Enmarcha.Samples.SemanticKernel.DocumentContentExtractor;

internal class Example
{
    private readonly Kernel kernel;
    private readonly IDocumentConnectorProvider documentConnectorProvider;
    private readonly IDocumentContentExtractor documentContentExtractor;

    public Example(Kernel kernel, IDocumentConnectorProvider documentConnectorProvider, IDocumentContentExtractor documentContentExtractor)
    {
        this.kernel = kernel;
        this.documentConnectorProvider = documentConnectorProvider;
        this.documentContentExtractor = documentContentExtractor;
    }

    public void ExtractDocumentContent()
    {
        Console.WriteLine("Please enter the path to the document you want to extract content from:");
        var filePath = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(filePath))
        {
            Console.WriteLine("Invalid file path. Please try again.");
            return;
        }

        var extension = Path.GetExtension(filePath);
        if (!documentConnectorProvider.SupportedFileExtension(extension))
        {
            Console.WriteLine($"The file extension is not supported.");
            return;
        }

        var stream = File.OpenRead(filePath);
        var documentChunks = documentContentExtractor.GetDocumentContent(stream, extension).ToList();

        // Print the extracted content
        foreach (var chunk in documentChunks)
        {
            Console.WriteLine(chunk);
            Console.WriteLine("------------");
        }

        Console.WriteLine($"Total chunks extracted: {documentChunks.Count}");
    }
}
