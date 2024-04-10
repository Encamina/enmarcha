using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Tests;

public class ExcelDocumentConnectorTests
{
    private const string ExcelFile = "TestFile.xlsx";
    private const string ExcelFileWithTwoWorksheets = "TestFileWithTwoWorksheets.xlsx";

    [Fact]
    public void ReadExcelFile_Succeeds()
    {
        var excelStream = GivenAnExcelStream(ExcelFile);
        var excelConnector = new ExcelDocumentConnector();

        var result = excelConnector.ReadText(excelStream);

        const string expectedResult = """
                                      "COLUMN 1","COLUMN 2","COLUMN 3"
                                      "Value 1-1","Value 1-2","Value 1-3"
                                      11,12,13
                                      """;

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void ReadExcelFile_WithCustomProperties_Succeeds()
    {
        var excelStream = GivenAnExcelStream(ExcelFile);
        var excelConnector = new ExcelDocumentConnector()
        {
            ColumnSeparator = "|",
            WithQuotes = false,
            WithWorksheetName = true,
            WorksheetTemplateName = (name) => $"### {name} ###"
        };

        var result = excelConnector.ReadText(excelStream);

        const string expectedResult = """
                                      ### Sheet Test 1 ###
                                      COLUMN 1|COLUMN 2|COLUMN 3
                                      Value 1-1|Value 1-2|Value 1-3
                                      11|12|13
                                      """;

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void ReadExcelFile_WithTwoWorksheets_Succeeds()
    {
        var excelStream = GivenAnExcelStream(ExcelFileWithTwoWorksheets);
        var excelConnector = new ExcelDocumentConnector();

        var result = excelConnector.ReadText(excelStream);

        const string expectedResult = """
                                      "COLUMN 1","COLUMN 2","COLUMN 3"
                                      "Value 1-1","Value 1-2","Value 1-3"
                                      11,12,13

                                      "COLUMN Worksheet 2"
                                      "Value Worksheet 1"
                                      """;

        Assert.Equal(expectedResult, result);
    }

    private static FileStream GivenAnExcelStream(string fileName)
    {
        return File.OpenRead($"{Directory.GetCurrentDirectory()}/TestUtilities/Files/{fileName}");
    }
}