using System.Text;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.AI.Abstractions;

using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Extracts the text from CSV and TSV files, taking into account the size of the chunks, duplicating the headers for each one. In this way we avoid the loss of context when chunking.
/// </summary>
public class CsvTsvDocumentConnector : IEnmarchaDocumentConnector
{
    private readonly ITextSplitter textSplitter;
    private readonly Func<string, int> lengthFunction;
    private TextSplitterOptions textSplitterOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvTsvDocumentConnector"/> class.
    /// </summary>
    /// <param name="textSplitter">A valid instance of <see cref="ITextSplitter"/> to use when extracting content from documents.</param>
    /// <param name="lengthFunction">Length function to use when extracting content from documents.</param>
    /// <param name="textSplitterOptions">Options for the text splitter.</param>
    public CsvTsvDocumentConnector(ITextSplitter textSplitter, Func<string, int> lengthFunction, IOptionsMonitor<TextSplitterOptions> textSplitterOptions)
    {
        this.textSplitter = textSplitter;
        this.lengthFunction = lengthFunction;

        this.textSplitterOptions = textSplitterOptions.CurrentValue;
        textSplitterOptions.OnChange(newOptions => this.textSplitterOptions = newOptions);
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> CompatibleFileFormats => [".CSV", ".TSV"];

    /// <summary>
    /// Gets the encoding used for reading the text from the stream.
    /// </summary>
    protected virtual Encoding Encoding => Encoding.UTF8;

    /// <inheritdoc/>
    public string ReadText(Stream stream)
    {
        Guard.IsNotNull(stream);

        using var streamReader = new StreamReader(stream, Encoding);
        var allText = streamReader.ReadToEnd().Trim();

        var firstEndOfLineIndex = GetFirstEndOfLineIndex(allText);

        if (firstEndOfLineIndex == -1)
        {
            return allText; // There is just one line. Nothing to do.
        }

        var headers = allText[..firstEndOfLineIndex];
        var content = allText[(firstEndOfLineIndex + 1)..];
        var headersLength = lengthFunction(headers);

        // Split the content into chunks. Leaving room to duplicate the header on each one
        var ajustedTextSpliterOptions = new TextSplitterOptions()
        {
            ChunkOverlap = textSplitterOptions.ChunkOverlap,
            ChunkSize = textSplitterOptions.ChunkSize - headersLength,
            Separators = textSplitterOptions.Separators,
        };
        var splittedContent = textSplitter.Split(content, lengthFunction, ajustedTextSpliterOptions);

        // Rebuild the text, duplicating the headers for each chunk.
        var sbResult = new StringBuilder();
        foreach (var contentChunk in splittedContent)
        {
            sbResult.AppendLine(headers);
            sbResult.AppendLine(contentChunk);

            sbResult.AppendLine(); // Add a blank line between records.
        }

        return sbResult.ToString().Trim();
    }

    /// <inheritdoc/>
    public virtual void Initialize(Stream stream)
    {
        // Intentionally not implemented to comply with the Liskov Substitution Principle...
    }

    /// <inheritdoc/>
    public virtual void AppendText(Stream stream, string text)
    {
        // Intentionally not implemented to comply with the Liskov Substitution Principle...
    }

    private static int GetFirstEndOfLineIndex(string text)
    {
        var newLineIndex = text.IndexOf("\r\n");

        if (newLineIndex == -1)
        {
            newLineIndex = text.IndexOf('\n');

            if (newLineIndex == -1)
            {
                newLineIndex = text.IndexOf('\r');
            }
        }

        return newLineIndex;
    }
}
