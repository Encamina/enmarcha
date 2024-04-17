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
    /// Gets a value indicating whether empty rows should be included in the result.
    /// </summary>
    public bool WithEmptyRows { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether empty columns should be included in the result.
    /// </summary>
    public bool WithEmptyColumns { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether cell values should be read with formatted.
    /// </summary>
    public bool WithFormattedValues { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether a separator should be added between the header and the data.
    /// </summary>
    public bool WithHeaderSeparator { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether hidden sheets should be included in the result.
    /// </summary>
    public bool WithHiddenWorksheets { get; init; }

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

        var excelDocument = ExcelDocument.Create(stream);

        var worksheets = excelDocument.Worksheets
            .Where(worksheet => WithHiddenWorksheets || !worksheet.IsHidden)
            .ToList();

        var rowSb = new StringBuilder();

        foreach (var worksheet in worksheets)
        {
            if (WithWorksheetName)
            {
                rowSb.AppendLine(WorksheetTemplateName(worksheet.Name));
            }

            var rows = worksheet.Rows;

            rows = WithEmptyColumns
                ? worksheet.Rows
                : RemoveEmptyColumnsFromRows(rows);

            rows = WithEmptyRows
                ? rows
                : rows.Where(row => row.Any(cell => !cell.IsEmpty)).ToList();

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

    private static IReadOnlyList<IReadOnlyList<Cell>> RemoveEmptyColumnsFromRows(IReadOnlyList<IReadOnlyList<Cell>> rows)
    {
        if (rows.Count == 0 || rows[0].Count == 0)
        {
            return rows;
        }

        var numColumns = rows[0].Count;
        var modifiedRows = rows.Select(row => row.ToList()).ToList();

        for (var columnIndex = 0; columnIndex < numColumns; columnIndex++)
        {
            var allCellsAreEmpty = modifiedRows.All(row => row[columnIndex].IsEmpty);

            if (allCellsAreEmpty)
            {
                // Remove the columnIndex column from all modified rows
                foreach (var row in modifiedRows)
                {
                    row.RemoveAt(columnIndex);
                }

                // After removing a column, adjust the index backwards
                columnIndex--;
                numColumns--;
            }
        }

        // Convert the modified rows back
        return modifiedRows.Select(row => row.AsReadOnly()).ToList().AsReadOnly();
    }

    private string GetCellTextValue(Cell cell)
    {
        var style = string.Empty;
        if (WithStyling)
        {
            style = cell.IsBold ? "**" : string.Empty;
            style += cell.IsItalic ? "*" : string.Empty;
        }

        return WithFormattedValues
            ? $"{style}{cell.FormattedText}{style}"
            : $"{style}{cell.Text}{style}";
    }
}
