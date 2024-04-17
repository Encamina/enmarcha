using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Models.Excel;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Tests;

public class ExcelDocumentTest
{
    private const string ExcelFile = "TestFile.xlsx";
    private const string ExcelFileWithTwoWorksheets = "TestFileWithTwoWorksheets.xlsx";
    private const string ExcelFileWithHiddenSheet = "TestFileWithHiddenSheet.xlsx";
    private const string ExcelFileWithFormatValues = "TestFileWithFormatValues.xlsx";
    private const string ExcelFileWithFontStyles = "TestFileWithFontStyle.xlsx";
    private const string ExcelFileWithEmptyRowAndColumn = "TestFileWithEmptyRowAndColumn.xlsx";

    [Fact]
    public void ReadExcelFile_With_MultipleWorksheets()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithTwoWorksheets);

        var result = ExcelDocument.Create(excelStream);

        Assert.Equal(2, result.Worksheets.Count);
    }

    [Fact]
    public void CreateExcelDocument_With_HiddenWorksheets()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithHiddenSheet);

        var result = ExcelDocument.Create(excelStream);
        
        Assert.Equal(2, result.Worksheets.Count);
        Assert.False(result.Worksheets[0].IsHidden);
        Assert.True(result.Worksheets[1].IsHidden);
    }

    [Fact]
    public void CreateExcelDocument_With_CellReferences()
    {
        var excelStream = GivenAnExcelStream(ExcelFile);

        var result = ExcelDocument.Create(excelStream);

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

        var result = ExcelDocument.Create(excelStream);

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

        var result = ExcelDocument.Create(excelStream);

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

        var result = ExcelDocument.Create(excelStream);

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
    public void ReadExcelFile_With_EmptyRowAndColumn_FillAllRangeUsed()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithEmptyRowAndColumn);

        var result = ExcelDocument.Create(excelStream);

        var worksheet = Assert.Single(result.Worksheets);

        Assert.Equal(6, worksheet.Rows.Count);

        Assert.Equal(6, worksheet.Rows[0].Count);
        Assert.False(worksheet.Rows[0][0].IsEmpty);
        Assert.False(worksheet.Rows[0][1].IsEmpty);
        Assert.True(worksheet.Rows[0][2].IsEmpty);
        Assert.True(worksheet.Rows[0][3].IsEmpty);
        Assert.True(worksheet.Rows[0][4].IsEmpty);
        Assert.True(worksheet.Rows[0][5].IsEmpty);

        Assert.Equal(6, worksheet.Rows[1].Count);
        Assert.False(worksheet.Rows[1][0].IsEmpty);
        Assert.False(worksheet.Rows[1][1].IsEmpty);
        Assert.True(worksheet.Rows[1][2].IsEmpty);
        Assert.True(worksheet.Rows[1][3].IsEmpty);
        Assert.True(worksheet.Rows[1][4].IsEmpty);
        Assert.True(worksheet.Rows[1][5].IsEmpty);

        Assert.Equal(6, worksheet.Rows[2].Count);
        Assert.True(worksheet.Rows[2][0].IsEmpty);
        Assert.False(worksheet.Rows[2][1].IsEmpty);
        Assert.True(worksheet.Rows[2][2].IsEmpty);
        Assert.True(worksheet.Rows[2][3].IsEmpty);
        Assert.True(worksheet.Rows[2][4].IsEmpty);
        Assert.True(worksheet.Rows[2][5].IsEmpty);

        Assert.Equal(6, worksheet.Rows[3].Count);
        Assert.True(worksheet.Rows[3][0].IsEmpty);
        Assert.True(worksheet.Rows[3][1].IsEmpty);
        Assert.True(worksheet.Rows[3][2].IsEmpty);
        Assert.True(worksheet.Rows[3][3].IsEmpty);
        Assert.True(worksheet.Rows[3][4].IsEmpty);
        Assert.True(worksheet.Rows[3][5].IsEmpty);

        Assert.Equal(6, worksheet.Rows[4].Count);
        Assert.True(worksheet.Rows[4][0].IsEmpty);
        Assert.True(worksheet.Rows[4][1].IsEmpty);
        Assert.True(worksheet.Rows[4][2].IsEmpty);
        Assert.False(worksheet.Rows[4][3].IsEmpty);
        Assert.False(worksheet.Rows[4][4].IsEmpty);
        Assert.False(worksheet.Rows[4][5].IsEmpty);

        Assert.Equal(6, worksheet.Rows[5].Count);
        Assert.True(worksheet.Rows[5][0].IsEmpty);
        Assert.True(worksheet.Rows[5][1].IsEmpty);
        Assert.True(worksheet.Rows[5][2].IsEmpty);
        Assert.False(worksheet.Rows[5][3].IsEmpty);
        Assert.False(worksheet.Rows[5][4].IsEmpty);
        Assert.True(worksheet.Rows[5][5].IsEmpty);
    }

    private static FileStream GivenAnExcelStream(string fileName)
    {
        return File.OpenRead($"{Directory.GetCurrentDirectory()}/TestUtilities/Files/{fileName}");
    }
}
