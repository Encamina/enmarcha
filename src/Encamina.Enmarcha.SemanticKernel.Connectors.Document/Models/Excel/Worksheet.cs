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

        var hiddenRows = excelLoadOptions.ExcludeHiddenRows ? GetHiddenRows(worksheet) : [];
        var hiddenColumns = excelLoadOptions.ExcludeHiddenColumns ? GetHiddenColumns(worksheet) : [];

        for (var row = startCell.Row; row <= endCell.Row; row++)
        {
            if (excelLoadOptions.ExcludeHiddenRows && hiddenRows.Contains(row))
            {
                continue;
            }

            var rowCells = new List<Cell>();

            for (var column = startCell.Column; column <= endCell.Column; column++)
            {
                if (excelLoadOptions.ExcludeHiddenColumns && hiddenColumns.Contains(column))
                {
                    continue;
                }

                var cellReference = CellReferenceConverter.CoordinatesToCellReference(row, column);

                var cell = cells.FirstOrDefault(c => c.CellReference == cellReference);

                rowCells.Add(cell != null
                    ? Cell.Create(cell, workbookPart)
                    : Cell.Empty(cellReference));
            }

            rows.Add(rowCells);
        }

        if(excelLoadOptions.MergeEmptyRowsRules != null)
        {
            rows = MergeEmptyRows(rows, excelLoadOptions.MergeEmptyRowsRules);
        }

        if(excelLoadOptions.MergeEmptyColumnsRules != null)
        {
            rows = MergeEmptyColumns(rows, excelLoadOptions.MergeEmptyColumnsRules);
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
    /// Merges empty rows based on the specified rules.
    /// </summary>
    /// <param name="rows">The list of rows to be processed.</param>
    /// <param name="mergeEmptyRowsRules">The rules for merging empty rows.</param>
    /// <returns>The list of rows after merging empty rows.</returns>
    private static List<List<Cell>> MergeEmptyRows(IReadOnlyList<List<Cell>> rows, MergeEmptyElementsRules mergeEmptyRowsRules)
    {
        // Identify ranges of consecutive empty rows to be merged
        var trimmedRanges = new List<(int Start, int Count)>();
        var start = 0;
        for (var i = 0; i < rows.Count; i++)
        {
            if (rows[i].All(r => r.IsNullOrWhiteSpace))
            {            
                // Found the start of a potential empty row range
                if (start == -1)
                {
                    start = i;
                }
            }
            else
            {
                // End of an empty row range, add it to trimmedRanges if it meets merge conditions
                if (start != -1 && i - start >= mergeEmptyRowsRules.MinimumElementsToMerge)
                {
                    trimmedRanges.Add((start + mergeEmptyRowsRules.ResultingElementsFromMerge, (i) - (start + mergeEmptyRowsRules.ResultingElementsFromMerge)));
                }
                start = -1;
            }
        }

        // Filter the identified empty row ranges from the rows list
        return rows.Where((_, index) => !trimmedRanges.Any(range => index >= range.Start && index < range.Start + range.Count)).ToList();
    }

    private static List<List<Cell>> MergeEmptyColumns(IReadOnlyList<List<Cell>> rows, MergeEmptyElementsRules mergeEmptyColumnsRules)
    {
        // Identify ranges of consecutive empty columns to be merged
        var trimmedRanges = new List<(int Start, int Count)>();
        var start = 0;
        for (var i = 0; i < rows[0].Count; i++)
        {
            if (rows.All(r => r[i].IsNullOrWhiteSpace))
            {
                // Found the start of a potential empty column range
                if (start == -1)
                {
                    start = i;
                }
            }
            else
            {
                // End of an empty column range, add it to trimmedRanges if it meets merge conditions
                if (start != -1 && i - start >= mergeEmptyColumnsRules.MinimumElementsToMerge)
                {
                    trimmedRanges.Add((start + mergeEmptyColumnsRules.ResultingElementsFromMerge, i - (start + mergeEmptyColumnsRules.ResultingElementsFromMerge)));
                }
                start = -1;
            }
        }

        // Reassign 'rows' with a new list of rows, where each row is a new list of cells excluding the cells in the ranges specified by trimmedRanges
        return rows.Select(row => row.Where((_, index) => !trimmedRanges.Any(range => index >= range.Start && index < range.Start + range.Count)).ToList()).ToList();
    }

    /// <summary>
    /// Gets the hidden rows in the worksheet.
    /// </summary>
    /// <param name="worksheet">The <see cref="WorkbookPart"/> object containing the rows.</param>
    /// <returns>A list of integers representing the indices of hidden rows.</returns>
    private static IReadOnlyList<int> GetHiddenRows(DocumentFormat.OpenXml.Spreadsheet.Worksheet worksheet)
    {
        return worksheet.Descendants<Row>()
            .Where((r) => r.Hidden != null && r.Hidden.Value && r.RowIndex?.Value is not null)
            .Select(r => (int)r.RowIndex.Value)
            .ToList();
    }

    /// <summary>
    /// Gets the hidden columns in the worksheet.
    /// </summary>
    /// <param name="worksheet">The <see cref="WorkbookPart"/> object containing the columns.</param>
    /// <returns>A list of integers representing the indices of hidden columns.</returns>
    private static IReadOnlyList<int> GetHiddenColumns(DocumentFormat.OpenXml.Spreadsheet.Worksheet worksheet)
    {
        return worksheet.Descendants<Column>()
            .Where(c => c.Hidden != null && c.Hidden.Value && c.Min != null && c.Max != null)
            .SelectMany(c => Enumerable.Range((int)c.Min.Value, (int)c.Max.Value - (int)c.Min.Value + 1))
            .ToList();
    }

    /// <summary>
    /// Removes empty columns from the given list of rows.
    /// </summary>
    /// <param name="rows">The list of rows where empty columns are to be removed.</param>
    /// <returns>The list of rows with empty columns removed.</returns>
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
