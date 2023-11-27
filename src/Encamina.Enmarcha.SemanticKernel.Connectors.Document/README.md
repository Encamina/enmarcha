# Semantic Kernel - Document Connectors

Document Connectors specializes in reading information from files in various formats and subsequently chunking it. The most typical use case is, within the context of generating document embeddings, reading information from a variety of file formats (pdf, docx, pptx, etc.) and chunks its content into smaller parts.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.SemanticKernel.Connectors.Document](https://www.nuget.org/packages/Encamina.Enmarcha.SemanticKernel.Connectors.Document) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.SemanticKernel.Connectors.Document

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.SemanticKernel.Connectors.Document](https://www.nuget.org/packages/Encamina.Enmarcha.SemanticKernel.Connectors.Document) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.SemanticKernel.Connectors.Document

## How to use

Starting from a `Program.cs` or a similar entry point file in your project, add the following code:
```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ...

services.AddDefaultDocumentContentExtractor();
```

This extension method will add the default implementation of the [IDocumentContentExtractor](../Encamina.Enmarcha.AI.Abstractions/IDocumentContentExtractor.cs) interface as a singleton. The default implementation is [DefaultDocumentContentExtractor](DefaultDocumentContentExtractor.cs). With this, we can resolve the `IDocumentContentExtractor` interface and obtain the chunks of a file:

### Construction injection

```csharp
public class MyClass
{
    private readonly IDocumentContentExtractor documentContentExtractor;

    public MyClass(IDocumentContentExtractor documentContentExtractor)
    {
        this.documentContentExtractor = documentContentExtractor;
    }

    public IEnumerable<string> GetPdfChunks()
    {
        using var file = File.OpenRead("example.pdf");

        var pdfChunks = documentContentExtractor.GetDocumentContent(file, ".pdf");

        return pdfChunks;
    }
}
```

### Service Provider

```csharp
var serviceProvider = services.BuildServiceProvider();
var documentContentExtractor = serviceProvider.GetRequiredService<IDocumentContentExtractor>();

using var file = File.OpenRead("example.pdf");
var fileChunks = documentContentExtractor.GetDocumentContent(file, ".pdf");
```

> For the above code to be fully functional, it is necessary to configure some additional services, specifically the [ITextSplitter](../Encamina.Enmarcha.AI.Abstractions/ITextSplitter.cs) interface and a [function to calculate the length of each chunk](../Encamina.Enmarcha.AI.Abstractions/ILengthFunctions.cs).

The previous code, based on the file extension, searches for a suitable [IDocumentConnector](https://github.com/microsoft/semantic-kernel/blob/76db027273371ea81e6db66afcb1d888cc53b459/dotnet/src/Plugins/Plugins.Document/IDocumentConnector.cs#L10) for the file type, processes the file to extract its text and finally, it uses an `ITextSplitter` to split the text into chunks.

### Details about the `IDocumentConnector`

The default implementation `DefaultDocumentContentExtractor`, uses the following `IDocumentConnectors`:

- [`WordDocumentConnector`](https://github.com/microsoft/semantic-kernel/blob/76db027273371ea81e6db66afcb1d888cc53b459/dotnet/src/Plugins/Plugins.Document/OpenXml/WordDocumentConnector.cs#L13): For *.docx* files, it extracts the text from the file by adding each paragraph on a new line.

- [`CleanPdfDocumentConnector`](Connectors/CleanPdfDocumentConnector.cs): For *.pdf* files, it extracts the raw text from the file (with all words separated by spaces) and removes common words, typically headers or footers that appear in at least 25% of the document.

- [`ParagraphPptxDocumentConnector`](Connectors/ParagraphPptxDocumentConnector.cs): For *.pptx* files, it extracts the text from the file, with one line per paragraph found in each slide.

- [`TxtDocumentConnector`](Connectors/TxtDocumentConnector.cs): For *.txt* files, it extracts the raw text from the file using UTF-8 as the character encoding.

- [`TxtDocumentConnector`](Connectors/TxtDocumentConnector.cs): For *.md* files, it extracts the raw text from the file using UTF-8 as the character encoding.

- [`VttDocumentConnector`](Connectors/VttDocumentConnector.cs): For *.vtt* files, it extracts the text from the subtitles while removing the timestamp marks. Use UTF-8 as the character encoding.

For other formats, it throws a `NotSupportedException`.

### Others available `IDocumentConnector`

- [`SlidePptxDocumentConnector`](Connectors/SlidePptxDocumentConnector.cs): For *.pptx* files, it extracts the text from the file with just one line for each slide found.

- [`PdfDocumentConnector`](Connectors/PdfDocumentConnector.cs): For *.pdf* files, it extracts the raw text from the file for each page (all words separated by spaces) and add a line break between the text of each page.

- [`PdfWithTocDocumentConnector`](Connectors/PdfWithTocDocumentConnector.cs): For *.pdf* files, it retrieve the Table of Contents and generates, for each Table of Contents item, a text with the section title, a colon mark (:), and the content text of the section (e.g. Title1: Content of the Title1 section). Add a line break between each section. The output format of the text is configurable with the `TocItemFormat` property. Additionally, remove common words, typically headers or footers that appear in at least 25% of the document.

- [`StrictFormatCleanPdfDocumentConnector`](Connectors/StrictFormatCleanPdfDocumentConnector.cs): For .pdf files, it extracts the text from the file and attempts to preserve the document's formatting, including paragraphs, titles, and other structural elements. Additionally, it removes common words, typically headers or footers that appear in at least 25% of the document, and it excludes non-horizontal text. During the text extraction process, an effort is made to retain the document's format; however, it is important to note that this process relies on OCR recognition, which is not perfect, and the results may vary depending on the quality of the PDF.

### Use your own `IDocumentConnector` 

To use your own `IDocumentConnectors`, you can use the base class [DocumentContentExtractorBase](DocumentContentExtractorBase.cs) and override the `GetDocumentConnector` method. This way, you can return your own `IDocumentConnectors` to handle a specific file format based on the file extension.
```csharp
public class MyCustomDocumentContentExtractor : DocumentContentExtractorBase
{
    public MyCustomDocumentContentExtractor(ITextSplitter textSplitter, Func<string, int> lengthFunction) : base(textSplitter, lengthFunction)
    {
    }

    protected override IDocumentConnector GetDocumentConnector(string fileExtension)
    {
        return fileExtension.ToUpperInvariant() switch
        {
            @".rtf" => new MyCustomRtfDocumentConnector(),
            @".pdf" => new PdfWithTocDocumentConnector(),
            @".txt" => new TxtDocumentConnector(Encoding.UTF8),
            _ => throw new NotSupportedException(fileExtension),
        };
    }
}
```

Don't forget to register it.

```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ...

// Now we use our own implementation
// services.AddDefaultDocumentContentExtractor();

services.AddSingleton<IDocumentContentExtractor, MyCustomDocumentContentExtractor>();
```

With this, you will be able to use the extractor you need for each type of file.