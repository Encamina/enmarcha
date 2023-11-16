namespace Encamina.Enmarcha.AspNet.Mvc.Formatters.Csv;

/// <summary>
/// Configuration options for a comma separated values (<c>.csv</c>) formmatter.
/// </summary>
public class CsvFormatterOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the comma separated values should include a header (line) or not.
    /// </summary>
    public bool UseHeader { get; set; } = true;

    /// <summary>
    /// Gets or sets the values' delimiter or separator. Usually a comma ('<c>,</c>') character, but it can be any
    /// other. Defaults to '<c>,</c>'.
    /// </summary>
    public char Delimiter { get; set; } = ',';

    /// <summary>
    /// Gets or sets the encoding, usually with the name registered with the Internet Assigned Numbers Authority (IANA).
    /// Defaults to <c>UTF-8</c> encoding (from <see cref="System.Text.Encoding.WebName"/>).
    /// </summary>
    public string Encoding { get; set; } = System.Text.Encoding.UTF8.WebName;
}
