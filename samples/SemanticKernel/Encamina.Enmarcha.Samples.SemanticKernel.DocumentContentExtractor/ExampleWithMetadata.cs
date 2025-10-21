using System.Diagnostics;

using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document;

using Microsoft.SemanticKernel;

namespace Encamina.Enmarcha.Samples.SemanticKernel.DocumentContentExtractor;

/// <summary>
/// Provides an example of how to extract document content with metadata using the document content enriched extractor.
/// </summary>
internal class ExampleWithMetadata
{
    private readonly Kernel kernel;
    private readonly IDocumentConnectorProvider documentConnectorProvider;
    private readonly IDocumentContentEnrichedExtractor enrichedDocumentContentExtractor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExampleWithMetadata"/> class.
    /// </summary>
    /// <param name="kernel">A valid instance of <see cref="Kernel"/>.</param>
    /// <param name="documentConnectorProvider">A valid instance of <see cref="IDocumentConnectorProvider"/>.</param>
    /// <param name="enrichedDocumentContentExtractor">A valid instance of <see cref="IDocumentContentEnrichedExtractor"/>.</param>
    public ExampleWithMetadata(Kernel kernel, IDocumentConnectorProvider documentConnectorProvider, IDocumentContentEnrichedExtractor enrichedDocumentContentExtractor)
    {
        this.kernel = kernel;
        this.documentConnectorProvider = documentConnectorProvider;
        this.enrichedDocumentContentExtractor = enrichedDocumentContentExtractor;
    }

    /// <summary>
    /// Extracts content with metadata from a document specified by the user.
    /// </summary>
    public void ExtractDocumentContentEnriched()
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
            Console.WriteLine($"The file extension '{extension}' is not supported.");
            return;
        }

        var stopwatch = Stopwatch.StartNew();

        using var stream = File.OpenRead(filePath);
        var markdownChunks = enrichedDocumentContentExtractor.GetDocumentContentWithMetadata(stream, extension).ToList();

        // Print all chunks except the last with separator
        foreach (var (metadata, text) in markdownChunks.SkipLast(1))
        {
            Console.WriteLine($"[CHUNK]\n\n{text}\n\n[END CHUNK]\n");
            Console.WriteLine("[METADATA]\n");

            if (metadata.Any())
            {
                foreach (var (key, value) in metadata)
                {
                    Console.WriteLine($"{key}: {value}");
                }
            }
            else
            {
                Console.WriteLine("(No metadata available)");
            }

            Console.WriteLine("\n[END METADATA]\n");
            Console.WriteLine("-----------------------------------\n");
        }

        // Print the last chunk without separator
        if (markdownChunks.Count != 0)
        {
            var (metadata, text) = markdownChunks.Last();
            Console.WriteLine($"[CHUNK]\n\n{text}\n\n[END CHUNK]\n");
            Console.WriteLine("[METADATA]\n");

            if (metadata.Any())
            {
                foreach (var (key, value) in metadata)
                {
                    Console.WriteLine($"{key}: {value}");
                }
            }
            else
            {
                Console.WriteLine("(No metadata available)");
            }

            Console.WriteLine("\n[END METADATA]\n");
        }

        stopwatch.Stop();

        // Final summary with double separator
        Console.WriteLine("-----------------------------------");
        Console.WriteLine("-----------------------------------\n");
        Console.WriteLine("[ADDITIONAL INFORMATION]\n");
        Console.WriteLine($"Total chunks extracted: {markdownChunks.Count}");
        Console.WriteLine($"Time elapsed: {stopwatch.Elapsed:hh\\:mm\\:ss}\n");
        Console.WriteLine("[END ADDITIONAL INFORMATION]");
    }
}
