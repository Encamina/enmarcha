using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Utils;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Models.Excel;

/// <summary>
/// Represents a worksheet in an Excel document.
/// </summary>
internal class Worksheet
{
    private readonly List<List<Cell>> rows = [];

    /// <summary>
    /// Gets the name of the worksheet.
    /// </summary>
    public string Name { get; private init; }

    /// <summary>
    /// Gets a value indicating whether the worksheet is hidden.
    /// </summary>
    public bool IsHidden { get; private init; }

    /// <summary>
    /// Gets the rows in the worksheet. Each row is a list of cells.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<Cell>> Rows => rows;

    /// <summary>
    /// Creates a Worksheet object from a Sheet object and a WorkbookPart object.
    /// </summary>
    /// <param name="sheet">The <see cref="Sheet"/> object representing the worksheet in the Excel document.</param>
    /// <param name="workbookPart">The <see cref="WorkbookPart"/> object containing the Sheet object.</param>
    /// <returns>A Worksheet object representing the worksheet in the Excel document.</returns>
    public static Worksheet Create(Sheet sheet, WorkbookPart workbookPart)
    {
        var worksheetResult = new Worksheet()
        {
            Name = sheet.Name,
            IsHidden = sheet.State != null && (sheet.State == SheetStateValues.Hidden || sheet.State == SheetStateValues.VeryHidden),
        };

        if (sheet.Id?.Value == null)
        {
            return worksheetResult;
        }

        var worksheet = (workbookPart.GetPartById(sheet.Id.Value) as WorksheetPart)?.Worksheet;
        var cells = worksheet?.GetFirstChild<SheetData>()?.Descendants<Row>().SelectMany(r => r.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>()).ToList();

        if (cells == null || cells.Count == 0)
        {
            return worksheetResult;
        }

        var (startCell, endCell) = GetRangeUsed(worksheet);

        for (var row = startCell.Row; row <= endCell.Row; row++)
        {
            var rowCells = new List<Cell>();

            for (var column = startCell.Column; column <= endCell.Column; column++)
            {
                var cellReference = CellReferenceConverter.CoordinatesToCellReference(row, column);

                var cell = cells.FirstOrDefault(c => c.CellReference == cellReference);

                rowCells.Add(cell != null
                    ? Cell.Create(cell, workbookPart)
                    : Cell.Empty(cellReference));
            }

            worksheetResult.rows.Add(rowCells);
        }

        return worksheetResult;
    }

    private static ((int Row, int Column) Start, (int Row, int Column) End) GetRangeUsed(DocumentFormat.OpenXml.Spreadsheet.Worksheet worksheet)
    {
        var sheetDimension = worksheet.GetFirstChild<SheetDimension>();
        var dimensionsReference = sheetDimension?.Reference?.Value;

        if (dimensionsReference == null)
        {
            return ((0, 0), (0, 0));
        }

        var range = dimensionsReference.Split(':').Select(CellReferenceConverter.CellReferenceToCoordinates).ToArray();

        return (range[0], range[1]);
    }
}
