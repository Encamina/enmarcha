using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Models.Excel;

/// <summary>
/// Represents an Excel document.
/// </summary>
internal class ExcelDocument
{
    /// <summary>
    /// Gets the worksheets in the Excel document.
    /// </summary>
    public List<Worksheet> Worksheets { get; } = [];

    /// <summary>
    /// Creates an ExcelDocument object from a stream.
    /// </summary>
    /// <param name="stream">The stream containing the Excel document data.</param>
    /// <returns>An ExcelDocument object representing the Excel document in the stream.</returns>
    public static ExcelDocument Create(Stream stream)
    {
        var excelDocumentResult = new ExcelDocument();

        using var doc = SpreadsheetDocument.Open(stream, false);

        var sheets = doc.WorkbookPart?.Workbook.Sheets?.Elements<Sheet>().ToList() ?? [];

        foreach (var sheet in sheets)
        {
            excelDocumentResult.Worksheets.Add(Worksheet.Create(sheet, doc.WorkbookPart));
        }

        return excelDocumentResult;
    }
}
