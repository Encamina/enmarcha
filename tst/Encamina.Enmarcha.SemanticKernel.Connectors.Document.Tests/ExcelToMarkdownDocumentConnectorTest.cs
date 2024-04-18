using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Tests;

public class ExcelToMarkdownDocumentConnectorTest
{
    private const string ExcelFile = "TestFile.xlsx";
    private const string ExcelFileWithTwoWorksheets = "TestFileWithTwoWorksheets.xlsx";
    private const string ExcelFileWithHiddenSheet = "TestFileWithHiddenSheet.xlsx";
    private const string ExcelFileWithFormatValues = "TestFileWithFormatValues.xlsx";
    private const string ExcelFileWithFontStyles = "TestFileWithFontStyle.xlsx";
    private const string ExcelFileWithEmptyRowAndColumn = "TestFileWithEmptyRowAndColumn.xlsx";
    private const string ExcelFileMiscellaneous = "TestFileMiscellaneous.xlsx";

    [Fact]
    public void CreateMarkdownTable_Succeeds()
    {
        var excelStream = GivenAnExcelStream(ExcelFile);
        var excelConnector = new ExcelToMarkdownDocumentConnector();

        var result = excelConnector.ReadText(excelStream);

        const string expectedResult = """
                                      |COLUMN 1|COLUMN 2|COLUMN 3|
                                      |---|---|---|
                                      |Value 1-1|Value 1-2|Value 1-3|
                                      |11|12|13|
                                      """;

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void CreateMarkdownTable_With_CustomProperties()
    {
        var excelStream = GivenAnExcelStream(ExcelFile);
        var excelConnector = new ExcelToMarkdownDocumentConnector()
        {
            WithWorksheetName = true,
            WorksheetTemplateName = (name) => $"# {name}",
            WithHeaderSeparator = false,
        };

        var result = excelConnector.ReadText(excelStream);

        const string expectedResult = """
                                      # Sheet Test 1
                                      |COLUMN 1|COLUMN 2|COLUMN 3|
                                      |Value 1-1|Value 1-2|Value 1-3|
                                      |11|12|13|
                                      """;

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void CreateMarkdownTable_With_MultipleWorksheets()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithTwoWorksheets);
        var excelConnector = new ExcelToMarkdownDocumentConnector()
        {
            WithHeaderSeparator = true,
            WorksheetSeparator = "---",
        };

        var result = excelConnector.ReadText(excelStream);

        const string expectedResult = """
                                      |COLUMN 1|COLUMN 2|COLUMN 3|
                                      |---|---|---|
                                      |Value 1-1|Value 1-2|Value 1-3|
                                      |11|12|13|
                                      ---
                                      |COLUMN Worksheet 2|
                                      |---|
                                      |Value Worksheet 1|
                                      """;

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void CreateMarkdownTable_With_HiddenSheet()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithHiddenSheet);
        var excelConnector = new ExcelToMarkdownDocumentConnector()
        {
            ExcelLoadOptions =             
            {
                LoadHiddenSheets = true,
            },
            WithHeaderSeparator = true,
            WorksheetSeparator = "---",
            WithWorksheetName = true,
            WorksheetTemplateName = (name) => $"# {name}",
        };

        var result = excelConnector.ReadText(excelStream);

        const string expectedResult = """
                                      # Sheet Test 1
                                      |COLUMN 1-1|
                                      |---|
                                      |Value 1-1|
                                      ---
                                      # Sheet Hidden
                                      |COLUMN 1-2|
                                      |---|
                                      |Value 1-2|
                                      """;

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void CreateMarkdownTable_Without_HiddenSheet()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithHiddenSheet);
        var excelConnector = new ExcelToMarkdownDocumentConnector()
        {
            ExcelLoadOptions =
            {
                LoadHiddenSheets = false,
            },
            WithHeaderSeparator = true,
            WorksheetSeparator = "---",
            WithWorksheetName = true,
            WorksheetTemplateName = (name) => $"# {name}",
        };

        var result = excelConnector.ReadText(excelStream);

        const string expectedResult = """
                                      # Sheet Test 1
                                      |COLUMN 1-1|
                                      |---|
                                      |Value 1-1|
                                      """;

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void CreateMarkdownTable_With_FormatValues()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithFormatValues);
        var excelConnector = new ExcelToMarkdownDocumentConnector()
        {
            WithFormattedValues = true,
        };

        var result = excelConnector.ReadText(excelStream);

        const string expectedResult = """
                                      |-24.4%|-24.0%|24.0%|
                                      |---|---|---|
                                      |This is a text|||
                                      |4,317.1 |4317.06||
                                      |Tuesday, April 16, 2024|4/16/2024|4/16/24 12:00 AM|
                                      |23:59:59|11:59:59 PM||
                                      """;

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void CreateMarkdownTable_Without_FormatValues()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithFormatValues);
        var excelConnector = new ExcelToMarkdownDocumentConnector()
        {
            WithFormattedValues = false,
        };

        var result = excelConnector.ReadText(excelStream);

        const string expectedResult = """
                                      |-0.24393949618802505|-0.24|0.24|
                                      |---|---|---|
                                      |This is a text|||
                                      |4317.0635001000001|4317.0635001000001||
                                      |45398|45398|45398|
                                      |0.99998842592592596|0.99998842592592596||
                                      """;

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void CreateMarkdownTable_With_FontStyle()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithFontStyles);
        var excelConnector = new ExcelToMarkdownDocumentConnector()
        {
            WithStyling = true,
        };

        var result = excelConnector.ReadText(excelStream);

        const string expectedResult = """
                                      |COLUMN 1|COLUMN 2|COLUMN 3|COLUMN 4|
                                      |---|---|---|---|
                                      |**This is bold**|*This is cursive*|***This is bold and cursive***|This has no style|
                                      """;

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void CreateMarkdownTable_Without_FontStyle()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithFontStyles);
        var excelConnector = new ExcelToMarkdownDocumentConnector()
        {
            WithStyling = false,
        };

        var result = excelConnector.ReadText(excelStream);

        const string expectedResult = """
                                      |COLUMN 1|COLUMN 2|COLUMN 3|COLUMN 4|
                                      |---|---|---|---|
                                      |This is bold|This is cursive|This is bold and cursive|This has no style|
                                      """;

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void CreateMarkdownTable_With_EmptyRowsAndColumns()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithEmptyRowAndColumn);
        var excelConnector = new ExcelToMarkdownDocumentConnector
        {
            ExcelLoadOptions =
            {
                ExcludeEmptyColumns = false,
                ExcludeEmptyRows = false
            }
        };

        var result = excelConnector.ReadText(excelStream);

        const string expectedResult = """
                                      |COLUMN 1|COLUMN 2|||||
                                      |---|---|---|---|---|---|
                                      |Value 1|Value 2|||||
                                      ||Value 3|||||
                                      |||||||
                                      ||||COLUMN 4|COLUMN 5|COLUMN 6|
                                      ||||Value 4|Value 5||
                                      """;

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void CreateMarkdownTable_Without_EmptyRows()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithEmptyRowAndColumn);
        var excelConnector = new ExcelToMarkdownDocumentConnector
        {
            ExcelLoadOptions =
            {
                ExcludeEmptyRows = true,
                ExcludeEmptyColumns = false,
            }
        };

        var result = excelConnector.ReadText(excelStream);

        const string expectedResult = """
                                      |COLUMN 1|COLUMN 2|||||
                                      |---|---|---|---|---|---|
                                      |Value 1|Value 2|||||
                                      ||Value 3|||||
                                      ||||COLUMN 4|COLUMN 5|COLUMN 6|
                                      ||||Value 4|Value 5||
                                      """;

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void CreateMarkdownTable_Without_EmptyColumns()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithEmptyRowAndColumn);
        var excelConnector = new ExcelToMarkdownDocumentConnector
        {
            ExcelLoadOptions =
            {
                ExcludeEmptyColumns = true,
                ExcludeEmptyRows = false,
            }
        };

        var result = excelConnector.ReadText(excelStream);

        const string expectedResult = """
                                      |COLUMN 1|COLUMN 2||||
                                      |---|---|---|---|---|
                                      |Value 1|Value 2||||
                                      ||Value 3||||
                                      ||||||
                                      |||COLUMN 4|COLUMN 5|COLUMN 6|
                                      |||Value 4|Value 5||
                                      """;

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void CreateMarkdownTable_Without_EmptyRowsAndColumns()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithEmptyRowAndColumn);
        var excelConnector = new ExcelToMarkdownDocumentConnector
        {
            ExcelLoadOptions =
            {
                ExcludeEmptyRows = true,
                ExcludeEmptyColumns = true,
            }
        };

        var result = excelConnector.ReadText(excelStream);

        const string expectedResult = """
                                      |COLUMN 1|COLUMN 2||||
                                      |---|---|---|---|---|
                                      |Value 1|Value 2||||
                                      ||Value 3||||
                                      |||COLUMN 4|COLUMN 5|COLUMN 6|
                                      |||Value 4|Value 5||
                                      """;

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("<br>", "This is a text<br>with several<br>line breaks")]
    [InlineData(" ", "This is a text with several line breaks")]
    public void CreateMarkdownTable_And_ReplaceLineBreaks(string lineBreakReplacement, string expectedReplacementText)
    {
        var excelStream = GivenAnExcelStream(ExcelFileMiscellaneous);
        var excelConnector = new ExcelToMarkdownDocumentConnector()
        {
            LineBreakReplacement = lineBreakReplacement,
        };

        var result = excelConnector.ReadText(excelStream);

        Assert.Contains(expectedReplacementText, result);
    }

    [Fact]
    public void CreateMarkdownTable_WithValidFontStyle_WhenExistsLeadingAndTrailingSpaces()
    {
        var excelStream = GivenAnExcelStream(ExcelFileMiscellaneous);
        var excelConnector = new ExcelToMarkdownDocumentConnector();

        var result = excelConnector.ReadText(excelStream);

        Assert.Contains("    **This is text with spaces at the beginning and end and in bold**     ", result);
        Assert.Contains("    *This is text with spaces at the beginning in italic*", result);
        Assert.Contains("***This is text with spaces at the beginning and end and in bold and italic***        ", result);
    }

    [Fact]
    public void CreateMarkdownTable_And_NotApplyFontStyleToEmptyOrWhiteSpaces()
    {
        var excelStream = GivenAnExcelStream(ExcelFileMiscellaneous);
        var excelConnector = new ExcelToMarkdownDocumentConnector();

        var result = excelConnector.ReadText(excelStream);

        Assert.DoesNotContain("|****|", result);
        Assert.DoesNotContain("|**|", result);
        Assert.DoesNotContain("|******|", result);
        Assert.DoesNotContain("|*** ***|", result);
        Assert.DoesNotContain("| ******|", result);
        Assert.DoesNotContain("|****** |", result);

    }

    private static FileStream GivenAnExcelStream(string fileName)
    {
        return File.OpenRead($"{Directory.GetCurrentDirectory()}/TestUtilities/Files/{fileName}");
    }
}
