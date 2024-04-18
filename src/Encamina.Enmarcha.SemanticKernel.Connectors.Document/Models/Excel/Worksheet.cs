using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Utils;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Models.Excel;

/// <summary>
/// Represents a worksheet in an Excel document.
/// </summary>
internal class Worksheet
{
    private List<List<Cell>> rows = [];

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
    /// <returns>A Worksheet object representing the worksheet in the Excel document.</returns>
    public static Worksheet Create(Sheet sheet)
    {
        var worksheetResult = new Worksheet()
        {
            Name = sheet.Name,
            IsHidden = sheet.State != null && (sheet.State == SheetStateValues.Hidden || sheet.State == SheetStateValues.VeryHidden),
        };
        
        return worksheetResult;
    }

    /// <summary>
    /// Loads the rows in the worksheet.
    /// </summary>
    /// <param name="sheet">The <see cref="Sheet"/> object representing the worksheet in the Excel document.</param>
    /// <param name="workbookPart">The <see cref="WorkbookPart"/> object containing the Sheet object.</param>
    /// <param name="excelLoadOptions">Options for loading the Excel document</param>
    public void LoadRows(Sheet sheet, WorkbookPart workbookPart, ExcelLoadOptions excelLoadOptions)
    {
        rows.Clear();

        if (sheet.Id?.Value == null)
        {
            return;
        }

        var worksheet = (workbookPart.GetPartById(sheet.Id.Value) as WorksheetPart)?.Worksheet;
        var cells = worksheet?.GetFirstChild<SheetData>()?.Descendants<Row>().SelectMany(r => r.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>()).ToList();

        if (cells == null || cells.Count == 0)
        {
            return;
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

            rows.Add(rowCells);
        }

        if (excelLoadOptions.ExcludeEmptyRows)
        {
            rows = rows.Where(row => row.Any(cell => !cell.IsNullOrWhiteSpace)).ToList();
        }

        if (excelLoadOptions.ExcludeEmptyColumns)
        {
            rows = RemoveEmptyColumnsFromRows(rows);
        }

        if (excelLoadOptions.LoadOnlyCellsRangeWithText)
        {
            rows = GetOnlyCellsRangeWithText(rows);
        }
    }
   
    /// <summary>
    /// Gets the range used in the worksheet.
    /// </summary>
    /// <param name="worksheet">The <see cref="WorkbookPart"/> object containing the SheetDimension object.</param>
    /// <returns></returns>
    private static ((int Row, int Column) Start, (int Row, int Column) End) GetRangeUsed(DocumentFormat.OpenXml.Spreadsheet.Worksheet worksheet)
    {
        var sheetDimension = worksheet.GetFirstChild<SheetDimension>();
        var dimensionsReference = sheetDimension?.Reference?.Value;

        if (dimensionsReference == null)
        {
            return ((0, 0), (0, 0));
        }

        var range = dimensionsReference.Split(':').Select(CellReferenceConverter.CellReferenceToCoordinates).ToArray();

        return range.Length == 1 
            ? (range[0], range[0]) 
            : (range[0], range[1]);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rows"></param>
    /// <returns></returns>
    private static List<List<Cell>> RemoveEmptyColumnsFromRows(List<List<Cell>> rows)
    {
        if (rows.Count == 0 || rows[0].Count == 0)
        {
            return rows;
        }

        var numColumns = rows[0].Count;
        var emptyColumnsIndex = new List<int>();

        for (var columnIndex = 0; columnIndex < numColumns; columnIndex++)
        {
            if (rows.All(row => row[columnIndex].IsNullOrWhiteSpace))
            {
                emptyColumnsIndex.Add(columnIndex);
            }
        }

        return rows.Select(row => row.Where((_, index) => !emptyColumnsIndex.Contains(index)).ToList()).ToList();
    }

    /// <summary>
    /// Retrieves a range of cells containing text from a two-dimensional grid of cells.
    /// </summary>
    /// <param name="rows">The grid of cells represented as a list of rows, where each row is a list of cells.</param>
    /// <returns>A list of rows, each containing a list of cells, representing the range of cells with text.</returns>
    private static List<List<Cell>> GetOnlyCellsRangeWithText(IList<List<Cell>> rows)
    {
        var firstNonEmptyRowIndex = rows.IndexOf(rows.FirstOrDefault(r => r.Any(c => !c.IsNullOrWhiteSpace)));
        var lastNonEmptyRowIndex = rows.IndexOf(rows.LastOrDefault(r => r.Any(c => !c.IsNullOrWhiteSpace)));

        var columnsRange = Enumerable.Range(0, rows[0].Count).ToList();
        
        var firstNonEmptyColumnIndex = columnsRange.FirstOrDefault(j => rows.Any(row => !row[j].IsNullOrWhiteSpace));
        var lastNonEmptyColumnIndex = columnsRange.LastOrDefault(j => rows.Any(row => !row[j].IsNullOrWhiteSpace));

        // Return only the rows and columns with non-empty cells
        return rows.Skip(firstNonEmptyRowIndex).Take(lastNonEmptyRowIndex - firstNonEmptyRowIndex + 1)
            .Select(row => row.Skip(firstNonEmptyColumnIndex).Take(lastNonEmptyColumnIndex - firstNonEmptyColumnIndex + 1).ToList())
            .ToList();
    }
}
