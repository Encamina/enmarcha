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
    /// <param name="excelLoadOptions">Options for loading the Excel document</param>
    /// <returns>An ExcelDocument object representing the Excel document in the stream.</returns>
    public static ExcelDocument Create(Stream stream, ExcelLoadOptions excelLoadOptions)
    {
        var excelDocumentResult = new ExcelDocument();

        using var doc = SpreadsheetDocument.Open(stream, false);

        var sheets = doc.WorkbookPart?.Workbook.Sheets?.Elements<Sheet>().ToList() ?? [];

        foreach (var sheet in sheets)
        {
            var worksheet = Worksheet.Create(sheet);

            if (excelLoadOptions.ExcludeHiddenSheets && worksheet.IsHidden)
            {
                continue;
            }

            worksheet.LoadRows(sheet, doc.WorkbookPart, excelLoadOptions);
            excelDocumentResult.Worksheets.Add(worksheet);
        }

        return excelDocumentResult;
    }
}
