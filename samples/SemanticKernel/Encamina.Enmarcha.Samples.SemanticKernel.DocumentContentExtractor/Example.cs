using System.Diagnostics;

using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document;

using Microsoft.SemanticKernel;

namespace Encamina.Enmarcha.Samples.SemanticKernel.DocumentContentExtractor;

internal class Example
{
    private readonly Kernel kernel;
    private readonly IDocumentConnectorProvider documentConnectorProvider;
    private readonly IDocumentContentExtractor documentContentExtractor;
    private readonly IDocumentContentEnrichedExtractor enrichedDocumentContentExtractor;

    public Example(Kernel kernel, IDocumentConnectorProvider documentConnectorProvider, IDocumentContentExtractor documentContentExtractor, IDocumentContentEnrichedExtractor enrichedDocumentContentExtractor)
    {
        this.kernel = kernel;
        this.documentConnectorProvider = documentConnectorProvider;
        this.documentContentExtractor = documentContentExtractor;
        this.enrichedDocumentContentExtractor = enrichedDocumentContentExtractor;
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
        var markdownChunks = enrichedDocumentContentExtractor.GetDocumentContent(stream, extension).ToList();

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
        Console.WriteLine($"Time elapsed: {stopwatch.Elapsed.TotalSeconds:F2} seconds\n");
        Console.WriteLine("[END ADDITIONAL INFORMATION]");
    }
}
