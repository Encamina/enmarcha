using System.Text;

using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Models.Excel;

using Microsoft.SemanticKernel.Plugins.Document;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Extracts text from an Excel file (<c>.xlsx</c>) and exports to Markdown table format.
/// </summary>
public class ExcelToMarkdownDocumentConnector : IDocumentConnector
{
    /// <summary>
    /// Gets the options for loading the Excel document.
    /// </summary>
    public ExcelLoadOptions ExcelLoadOptions { get; } = new()
    {
        LoadHiddenSheets = false,
        LoadOnlyCellsRangeWithText = true,
        ExcludeEmptyColumns = false,
        ExcludeEmptyRows = false,
    };

    /// <summary>
    /// Gets the value used to replace line breaks.
    /// </summary>
    public string LineBreakReplacement { get; init; } = "<br>";

    /// <summary>
    /// Gets a value indicating whether cell values should be read with formatted.
    /// </summary>
    public bool WithFormattedValues { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether a separator should be added between the header and the data.
    /// </summary>
    public bool WithHeaderSeparator { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether styling (e.g., bold, italic) should be preserved.
    /// </summary>
    public bool WithStyling { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the worksheet name should be included in the output.
    /// </summary>
    public bool WithWorksheetName { get; init; }

    /// <summary>
    /// Gets the worksheet separator used when reading the Excel document.
    /// </summary>
    public string WorksheetSeparator { get; init; } = "---";

    /// <summary>
    /// Gets a value indicating whether the worksheet name should be included in the output.
    /// </summary>
    public Func<string, string> WorksheetTemplateName { get; init; } = (worksheetName) => $"# {worksheetName}:";

    /// <inheritdoc/>
    public string ReadText(Stream stream)
    {
        var resultSb = new StringBuilder();

        var excelDocument = ExcelDocument.Create(stream, ExcelLoadOptions);

        var worksheets = excelDocument.Worksheets.ToList();

        var rowSb = new StringBuilder();

        foreach (var worksheet in worksheets)
        {
            if (WithWorksheetName)
            {
                rowSb.AppendLine(WorksheetTemplateName(worksheet.Name));
            }

            var rows = worksheet.Rows;

            foreach (var row in rows)
            {
                var rowTexts = row.Select(GetCellTextValue).ToList();

                rowSb.AppendLine($"|{string.Join("|", rowTexts)}|");

                // Adds a separator between the header and the data
                if (WithHeaderSeparator && rows[0].Equals(row))
                {
                    rowSb.AppendLine($"|{string.Join("|", row.Select(_ => "---"))}|");
                }
            }

            resultSb.AppendLine(rowSb.ToString().Trim());

            if (worksheet != worksheets.Last())
            {
                resultSb.Append(WorksheetSeparator).AppendLine();
            }

            rowSb.Clear();
        }

        return resultSb.ToString().Trim();
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
    
    private string GetCellTextValue(Cell cell)
    {
        // Get the cell value with or without formatting
        var cellValue = WithFormattedValues ? cell.FormattedText : cell.Text;
        
        // Replace line breaks with the specified replacement
        cellValue = cellValue?.ReplaceLineEndings(LineBreakReplacement);

        // Apply styles to the cell value
        if (WithStyling)
        {
            cellValue = ApplyStyles(cellValue, cell.IsBold, cell.IsItalic);
        }

        return cellValue;
    }

    private static string ApplyStyles(string value, bool bold, bool italic)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var style = bold ? "**" : string.Empty;
        style += italic ? "*" : string.Empty;

        if(string.IsNullOrEmpty(style))
        {
            return value;
        }

        // Find the index of the first non-space character
        var firstNonSpaceIndex = value.TakeWhile(char.IsWhiteSpace).Count();

        // Apply styles at the beginning of the text
        value = value.Insert(firstNonSpaceIndex, style);

        // Find the index of the last non-space character
        var lastNonSpaceIndex = value.Length - value.Reverse().TakeWhile(char.IsWhiteSpace).Count();

        // Apply styles at the end of the text
        value = value.Insert(lastNonSpaceIndex, style);

        return value;
    }
}
