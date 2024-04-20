using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Models.Excel;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Tests;

public class ExcelDocumentTest
{
    private const string ExcelFile = "Test.xlsx";
    private const string ExcelFileWithTwoWorksheets = "TwoWorksheets.xlsx";
    private const string ExcelFileWithHiddenSheet = "HiddenSheet.xlsx";
    private const string ExcelFileWithFormatValues = "FormatValues.xlsx";
    private const string ExcelFileWithFontStyles = "FontStyle.xlsx";
    private const string ExcelFileWithEmptyRowAndColumn = "EmptyRowAndColumn.xlsx";
    private const string ExcelFileWithRangeWithEmptyCells = "RangeWithEmptyCells.xlsx";
    private const string ExcelFileWithHiddenRowsAndColumns = "HiddenRowsAndColumns.xlsx";
    private const string ExcelFileLargeEmptyRowsAndColumns = "LargeEmptyRowsAndColumns.xlsx";

    [Fact]
    public void CreateExcelDocument_With_MultipleWorksheets()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithTwoWorksheets);
        var excelOptions = new ExcelLoadOptions();

        var result = ExcelDocument.Create(excelStream, excelOptions);

        Assert.Equal(2, result.Worksheets.Count);
    }

    [Fact]
    public void CreateExcelDocument_Without_HiddenWorksheets()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithHiddenSheet);
        var excelOptions = new ExcelLoadOptions() { ExcludeHiddenSheets = true };

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);
        Assert.False(worksheet.IsHidden);
    }

    [Fact]
    public void CreateExcelDocument_With_HiddenWorksheets()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithHiddenSheet);
        var excelOptions = new ExcelLoadOptions() { ExcludeHiddenSheets = false };

        var result = ExcelDocument.Create(excelStream, excelOptions);

        Assert.Equal(2, result.Worksheets.Count);
        Assert.False(result.Worksheets[0].IsHidden);
        Assert.True(result.Worksheets[1].IsHidden);
    }

    [Fact]
    public void CreateExcelDocument_With_CellReferences()
    {
        var excelStream = GivenAnExcelStream(ExcelFile);
        var excelOptions = new ExcelLoadOptions();

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        Assert.Equal("A1", worksheet.Rows[0][0].Reference);
        Assert.Equal("B1", worksheet.Rows[0][1].Reference);
        Assert.Equal("C1", worksheet.Rows[0][2].Reference);

        Assert.Equal("A2", worksheet.Rows[1][0].Reference);
        Assert.Equal("B2", worksheet.Rows[1][1].Reference);
        Assert.Equal("C2", worksheet.Rows[1][2].Reference);

        Assert.Equal("A3", worksheet.Rows[2][0].Reference);
        Assert.Equal("B3", worksheet.Rows[2][1].Reference);
        Assert.Equal("C3", worksheet.Rows[2][2].Reference);
    }

    [Fact]
    public void CreateExcelDocument_With_CellFontProperties()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithFontStyles);
        var excelOptions = new ExcelLoadOptions();

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        Assert.False(worksheet.Rows[0][0].IsBold);
        Assert.False(worksheet.Rows[0][0].IsItalic);
        Assert.False(worksheet.Rows[0][1].IsBold);
        Assert.False(worksheet.Rows[0][1].IsItalic);
        Assert.False(worksheet.Rows[0][2].IsBold);
        Assert.False(worksheet.Rows[0][2].IsItalic);

        Assert.True(worksheet.Rows[1][0].IsBold);
        Assert.False(worksheet.Rows[1][0].IsItalic);
        Assert.False(worksheet.Rows[1][1].IsBold);
        Assert.True(worksheet.Rows[1][1].IsItalic);
        Assert.True(worksheet.Rows[1][2].IsBold);
        Assert.True(worksheet.Rows[1][2].IsItalic);
    }

    [Fact]
    public void CreateExcelDocument_With_CellFormattedTexts()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithFormatValues);
        var excelOptions = new ExcelLoadOptions();

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        Assert.Equal("-24.4%", worksheet.Rows[0][0].FormattedText);
        Assert.Equal("-24.0%", worksheet.Rows[0][1].FormattedText);
        Assert.Equal("24.0%", worksheet.Rows[0][2].FormattedText);

        Assert.Equal("This is a text", worksheet.Rows[1][0].FormattedText);

        Assert.Equal("4,317.1 ", worksheet.Rows[2][0].FormattedText);
        Assert.Equal("4317.06", worksheet.Rows[2][1].FormattedText);

        Assert.Equal("Tuesday, April 16, 2024", worksheet.Rows[3][0].FormattedText);
        Assert.Equal("4/16/2024", worksheet.Rows[3][1].FormattedText);
        Assert.Equal("4/16/24 12:00 AM", worksheet.Rows[3][2].FormattedText);

        Assert.Equal("23:59:59", worksheet.Rows[4][0].FormattedText);
        Assert.Equal("11:59:59 PM", worksheet.Rows[4][1].FormattedText);
    }

    [Fact]
    public void CreateExcelDocument_With_CellRawTexts()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithFormatValues);
        var excelOptions = new ExcelLoadOptions();

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        Assert.Equal("-0.24393949618802505", worksheet.Rows[0][0].Text);
        Assert.Equal("-0.24", worksheet.Rows[0][1].Text);
        Assert.Equal("0.24", worksheet.Rows[0][2].Text);

        Assert.Equal("This is a text", worksheet.Rows[1][0].Text);

        Assert.Equal("4317.0635001000001", worksheet.Rows[2][0].Text);
        Assert.Equal("4317.0635001000001", worksheet.Rows[2][1].Text);

        // The dates in Excel are expressed as the number of days since 1900-01-01. That's why the number 45398 appears where the date 4/16/204 is in Excel.
        Assert.Equal("45398", worksheet.Rows[3][0].Text);
        Assert.Equal("45398", worksheet.Rows[3][1].Text);
        Assert.Equal("45398", worksheet.Rows[3][2].Text);

        // The time in Excel is expressed as the fraction of a day. That's why the number 0.999988425925926 appears where the time 23:59:59 is in Excel.
        Assert.Equal("0.99998842592592596", worksheet.Rows[4][0].Text);
        Assert.Equal("0.99998842592592596", worksheet.Rows[4][1].Text);
    }

    [Fact]
    public void CreateExcelDocument_With_EmptyRowAndColumn_FillAllRangeUsed()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithEmptyRowAndColumn);
        var excelOptions = new ExcelLoadOptions();

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        Assert.Equal(6, worksheet.Rows.Count);
        Assert.All(worksheet.Rows, row => Assert.Equal(6, row.Count));

        var firstCell = worksheet.Rows.First().First();
        Assert.Equal("B2", firstCell.Reference);
        Assert.False(firstCell.IsNullOrWhiteSpace);

        var lastCell = worksheet.Rows.Last().Last();
        Assert.Equal("G7", lastCell.Reference);
        Assert.True(lastCell.IsNullOrWhiteSpace);
    }

    [Fact]
    public void CreateExcelDocument_With_EmptyRange_FillOnlyUsedCellsRange()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithRangeWithEmptyCells);
        var excelOptions = new ExcelLoadOptions()
        {
            LoadOnlyCellsRangeWithText = true,
        };

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        Assert.Equal(2, worksheet.Rows.Count);
        Assert.All(worksheet.Rows, row => Assert.Equal(3, row.Count));

        Assert.Equal("C3", worksheet.Rows.First().First().Reference);
        Assert.Equal("E4", worksheet.Rows.Last().Last().Reference);
    }

    [Fact]
    public void CreateExcelDocument_With_EmptyRange_FillAllUsedCellsRange()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithRangeWithEmptyCells);
        var excelOptions = new ExcelLoadOptions()
        {
            LoadOnlyCellsRangeWithText = false,
        };

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        Assert.Equal(19, worksheet.Rows.Count);
        Assert.All(worksheet.Rows, row => Assert.Equal(16, row.Count));

        Assert.Equal("A1", worksheet.Rows.First().First().Reference);
        Assert.Equal("P19", worksheet.Rows.Last().Last().Reference);
    }

    [Fact]
    public void CreateExcelDocument_With_EmptyRowsAndColumns()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithEmptyRowAndColumn);
        var excelOptions = new ExcelLoadOptions()
        {
            ExcludeEmptyColumns = false,
            ExcludeEmptyRows = false,
        };

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        Assert.Equal(6, worksheet.Rows.Count);
        Assert.All(worksheet.Rows, row => Assert.Equal(6, row.Count));

        Assert.Equal("B2", worksheet.Rows.First().First().Reference);
        Assert.Equal("G7", worksheet.Rows.Last().Last().Reference);
    }

    [Fact]
    public void CreateExcelDocument_Without_EmptyRows()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithEmptyRowAndColumn);
        var excelOptions = new ExcelLoadOptions()
        {
            ExcludeEmptyRows = true,
            ExcludeEmptyColumns = false,
        };

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        Assert.Equal(5, worksheet.Rows.Count);
        Assert.All(worksheet.Rows, row => Assert.Equal(6, row.Count));

        Assert.Equal("B2", worksheet.Rows.First().First().Reference);
        Assert.Equal("G7", worksheet.Rows.Last().Last().Reference);
    }

    [Fact]
    public void CreateExcelDocument_Without_EmptyColumns()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithEmptyRowAndColumn);
        var excelOptions = new ExcelLoadOptions()
        {
            ExcludeEmptyColumns = true,
            ExcludeEmptyRows = false,
        };

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        Assert.Equal(6, worksheet.Rows.Count);
        Assert.All(worksheet.Rows, row => Assert.Equal(5, row.Count));

        Assert.Equal("B2", worksheet.Rows.First().First().Reference);
        Assert.Equal("G7", worksheet.Rows.Last().Last().Reference);
    }

    [Fact]
    public void CreateExcelDocument_Without_EmptyRowsAndColumns()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithEmptyRowAndColumn);
        var excelOptions = new ExcelLoadOptions()
        {
            ExcludeEmptyColumns = true,
            ExcludeEmptyRows = true,
        };

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        Assert.Equal(5, worksheet.Rows.Count);
        Assert.All(worksheet.Rows, row => Assert.Equal(5, row.Count));

        Assert.Equal("B2", worksheet.Rows.First().First().Reference);
        Assert.Equal("G7", worksheet.Rows.Last().Last().Reference);
    }

    [Fact]
    public void CreateExcelDocument_Without_HiddenRows()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithHiddenRowsAndColumns);
        var excelOptions = new ExcelLoadOptions()
        {
            ExcludeHiddenRows = true,
        };

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        var allCellTexts = worksheet.Rows.SelectMany(r => r.Select(c => c.Text)).ToList();
        Assert.DoesNotContain("This is a hidden row", allCellTexts);
        Assert.DoesNotContain("This is another hidden row", allCellTexts);
        Assert.DoesNotContain("Another hidden row", allCellTexts);
    }

    [Fact]
    public void CreateExcelDocument_With_HiddenRows()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithHiddenRowsAndColumns);
        var excelOptions = new ExcelLoadOptions()
        {
            ExcludeHiddenRows = false,
        };

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        var allCellTexts = worksheet.Rows.SelectMany(r => r.Select(c => c.Text)).ToList();
        Assert.Contains("This is a hidden row", allCellTexts);
        Assert.Contains("This is another hidden row", allCellTexts);
        Assert.Contains("Another hidden row", allCellTexts);
    }

    [Fact]
    public void CreateExcelDocument_Without_HiddenColumns()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithHiddenRowsAndColumns);
        var excelOptions = new ExcelLoadOptions()
        {
            ExcludeHiddenColumns = true,
        };

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        var allCellTexts = worksheet.Rows.SelectMany(r => r.Select(c => c.Text)).ToList();
        Assert.DoesNotContain("This is a hidden column", allCellTexts);
        Assert.DoesNotContain("This is another hidden column", allCellTexts);
        Assert.DoesNotContain("Another hidden column", allCellTexts);
    }

    [Fact]
    public void CreateExcelDocument_With_HiddenColumns()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithHiddenRowsAndColumns);
        var excelOptions = new ExcelLoadOptions()
        {
            ExcludeHiddenColumns = false,
        };

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        var allCellTexts = worksheet.Rows.SelectMany(r => r.Select(c => c.Text)).ToList();
        Assert.Contains("This is a hidden column", allCellTexts);
        Assert.Contains("This is another hidden column", allCellTexts);
        Assert.Contains("Another hidden column", allCellTexts);
    }

    [Fact]
    public void CreateExcelDocument_With_CompressedEmptyRows()
    {
        var excelStream = GivenAnExcelStream(ExcelFileLargeEmptyRowsAndColumns);
        var excelOptions = new ExcelLoadOptions()
        {
            MergeEmptyRowsRules = new MergeEmptyElementsRules
            {
                MinimumElementsToMerge = 3,
                ResultingElementsFromMerge = 2,
            },
        };

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        Assert.Equal("x", worksheet.Rows[5][18].Text);
        Assert.Equal("xx", worksheet.Rows[8][16].Text);
        Assert.Equal("xxx", worksheet.Rows[11][11].Text);
    }

    [Fact]
    public void CreateExcelDocument_With_CompressedEmptyColumns()
    {
        var excelStream = GivenAnExcelStream(ExcelFileLargeEmptyRowsAndColumns);
        var excelOptions = new ExcelLoadOptions()
        {
            MergeEmptyColumnsRules = new MergeEmptyElementsRules
            {
                MinimumElementsToMerge = 3,
                ResultingElementsFromMerge = 2,
            },
        };

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        Assert.Equal("x", worksheet.Rows[5][9].Text);
        Assert.Equal("xx", worksheet.Rows[15][7].Text);
        Assert.Equal("xxx", worksheet.Rows[27][4].Text);
    }

    [Fact]
    public void CreateExcelDocument_With_CompressedEmptyColumnsAndRows()
    {
        var excelStream = GivenAnExcelStream(ExcelFileLargeEmptyRowsAndColumns);
        var excelOptions = new ExcelLoadOptions()
        {
            MergeEmptyColumnsRules = new MergeEmptyElementsRules
            {
                MinimumElementsToMerge = 3,
                ResultingElementsFromMerge = 2,
            },
            MergeEmptyRowsRules = new MergeEmptyElementsRules
            {
                MinimumElementsToMerge = 3,
                ResultingElementsFromMerge = 2,
            },
        };

        var result = ExcelDocument.Create(excelStream, excelOptions);

        var worksheet = Assert.Single(result.Worksheets);

        Assert.Equal("x", worksheet.Rows[5][9].Text);
        Assert.Equal("xx", worksheet.Rows[8][7].Text);
        Assert.Equal("xxx", worksheet.Rows[11][4].Text);
    }

    private static FileStream GivenAnExcelStream(string fileName)
    {
        return File.OpenRead($"{Directory.GetCurrentDirectory()}/TestUtilities/ExcelFiles/{fileName}");
    }
}
