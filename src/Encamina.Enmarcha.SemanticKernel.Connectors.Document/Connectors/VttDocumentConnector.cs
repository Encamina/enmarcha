// Ignore Spelling: vtt

using System.Text;
using System.Text.RegularExpressions;

using CommunityToolkit.Diagnostics;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Document connector for Video Text Tracks (<c>.vtt</c>) files.
/// </summary>
public sealed class VttDocumentConnector : IEnmarchaDocumentConnector
{
    private static readonly Regex PatternRegex
        = new(@"\d+\n\d{2}:\d{2}:\d{2}\.\d{3} --> \d{2}:\d{2}:\d{2}\.\d{3}\n(.+?)(?=\n\d+\n\d{2}:\d{2}:\d{2}\.\d{3} -->|\n\n|$)", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

    private readonly Encoding encoding;

    /// <summary>
    /// Initializes a new instance of the <see cref="VttDocumentConnector"/> class.
    /// </summary>
    /// <param name="encoding">The encoding to use when reading the file.</param>
    public VttDocumentConnector(Encoding encoding)
    {
        this.encoding = encoding;
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> CompatibleFileFormats => [".VTT"];

    /// <inheritdoc/>
    public string ReadText(Stream stream)
    {
        Guard.IsNotNull(stream);

        using var reader = new StreamReader(stream, encoding);
        var input = reader.ReadToEnd();

        var result = new StringBuilder();

        foreach (var match in PatternRegex.Matches(input).Cast<Match>())
        {
            result.Append(match.Groups[1].Value).Append(' ');
        }

        return result.ToString().Trim();
    }

    /// <inheritdoc/>
    public void Initialize(Stream stream)
    {
        // Intentionally not implemented to comply with the Liskov Substitution Principle...
    }

    /// <inheritdoc/>
    public void AppendText(Stream stream, string text)
    {
        // Intentionally not implemented to comply with the Liskov Substitution Principle...
    }
}
