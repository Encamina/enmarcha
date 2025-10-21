using System.Diagnostics;

using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document;

using Microsoft.SemanticKernel;

namespace Encamina.Enmarcha.Samples.SemanticKernel.DocumentContentExtractor;

/// <summary>
/// Provides an example of how to extract document content using the document content extractor.
/// </summary>
internal class Example
{
    private readonly Kernel kernel;
    private readonly IDocumentConnectorProvider documentConnectorProvider;
    private readonly IDocumentContentExtractor documentContentExtractor;

    /// <summary>
    /// Initializes a new instance of the <see cref="Example"/> class.
    /// </summary>
    /// <param name="kernel">A valid instance of <see cref="Kernel"/>.</param>
    /// <param name="documentConnectorProvider">A valid instance of <see cref="IDocumentConnectorProvider"/>.</param>
    /// <param name="documentContentExtractor">A valid instance of <see cref="IDocumentContentExtractor"/>.</param>
    public Example(Kernel kernel, IDocumentConnectorProvider documentConnectorProvider, IDocumentContentExtractor documentContentExtractor)
    {
        this.kernel = kernel;
        this.documentConnectorProvider = documentConnectorProvider;
        this.documentContentExtractor = documentContentExtractor;
    }

    /// <summary>
    /// Extracts content from a document specified by the user.
    /// </summary>
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

        var stopwatch = Stopwatch.StartNew();

        using var stream = File.OpenRead(filePath);
        var documentChunks = documentContentExtractor.GetDocumentContent(stream, extension).ToList();

        // Print all chunks except the last with separator
        foreach (var chunk in documentChunks.SkipLast(1))
        {
            Console.WriteLine($"[CHUNK]\n\n{chunk}\n\n[END CHUNK]\n");
            Console.WriteLine("-----------------------------------\n");
        }

        // Print the last chunk without separator
        if (documentChunks.Count != 0)
        {
            var lastChunk = documentChunks.Last();
            Console.WriteLine($"[CHUNK]\n\n{lastChunk}\n\n[END CHUNK]\n");
        }

        stopwatch.Stop();

        // Final summary with double separator
        Console.WriteLine("-----------------------------------");
        Console.WriteLine("-----------------------------------\n");
        Console.WriteLine("[ADDITIONAL INFORMATION]\n");
        Console.WriteLine($"Total chunks extracted: {documentChunks.Count}");
        Console.WriteLine($"Time elapsed: {stopwatch.Elapsed:hh\\:mm\\:ss}\n");
        Console.WriteLine("[END ADDITIONAL INFORMATION]");
    }
}
