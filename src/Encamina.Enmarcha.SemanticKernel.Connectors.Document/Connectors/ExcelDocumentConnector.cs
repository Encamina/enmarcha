using System.Text;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using Microsoft.SemanticKernel.Plugins.Document;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Extracts text from an Excel file (<c>.xlsx, .xlsm</c>), one line per row found in each worksheet.
/// </summary>
public class ExcelDocumentConnector : IDocumentConnector
{
    /// <summary>
    /// Gets the column separator used when reading the Excel document.
    /// </summary>
    public string ColumnSeparator { get; init; } = ",";

    /// <summary>
    /// Gets a value indicating whether text values should be surrounded with quotes.
    /// </summary>
    public bool WithQuotes { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether the worksheet name should be included in the output.
    /// </summary>
    public bool WithWorksheetName { get; init; }

    /// <summary>
    /// Gets a value indicating whether the worksheet name should be included in the output.
    /// </summary>
    public Func<string, string> WorksheetTemplateName { get; init; } = (worksheetName) => $"{worksheetName}:";

    /// <inheritdoc/>
    public string ReadText(Stream stream)
    {
        var resultSb = new StringBuilder();

        using var doc = SpreadsheetDocument.Open(stream, false);
        var sheets = doc.WorkbookPart?.Workbook.Sheets?.OfType<Sheet>().ToList();
        if (sheets == null)
        {
            return resultSb.ToString();
        }

        var rowSb = new StringBuilder();
        var sharedStrings = doc.WorkbookPart?.SharedStringTablePart?.SharedStringTable.Elements<SharedStringItem>().ToList() ?? [];

        foreach (var sheet in sheets)
        {
            if (sheet.Id?.Value == null)
            {
                continue;
            }

            var worksheet = (doc.WorkbookPart!.GetPartById(sheet.Id.Value) as WorksheetPart)?.Worksheet;
            var rows = worksheet?.GetFirstChild<SheetData>()?.Descendants<Row>();

            if (rows == null)
            {
                continue;
            }

            if (WithWorksheetName)
            {
                rowSb.AppendLine(WorksheetTemplateName(sheet.Name));
            }

            foreach (var row in rows)
            {
                var rowValue = GetRowValue(row, sharedStrings);

                rowSb.AppendLine(rowValue);
            }

            resultSb.AppendLine(rowSb.ToString().Trim())
                    .AppendLine();

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

    private static (string Value, bool IsText) GetCellValue(IEnumerable<SharedStringItem> sharedStrings, CellType cell)
    {
        if (cell.DataType?.Value == CellValues.SharedString && int.TryParse(cell.InnerText, out var index))
        {
            return (sharedStrings.ElementAt(index).InnerText, true);
        }

        return (cell.CellValue?.InnerText, false);
    }

    private string GetRowValue(OpenXmlElement row, IReadOnlyCollection<SharedStringItem> sharedStrings)
    {
        var cells = row.Descendants<Cell>().Where(r => r.CellValue != null).ToList();

        var rowSb = new StringBuilder();
        for (var i = 0; i < cells.Count; i++)
        {
            var cellValue = GetCellValue(sharedStrings, cells[i]);

            if (WithQuotes && cellValue.IsText)
            {
                rowSb.Append('"')
                    .Append(cellValue.Value)
                    .Append('"');
            }
            else
            {
                rowSb.Append(cellValue.Value);
            }

            if (i < cells.Count - 1)
            {
                rowSb.Append(ColumnSeparator);
            }
        }

        return rowSb.ToString();
    }
}
