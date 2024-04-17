using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Utils;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Tests;

public class CellReferenceConverterTests
{
    [Theory]
    [InlineData(1, 1, "A1")]
    [InlineData(2, 2, "B2")]
    [InlineData(5, 27, "AA5")]
    [InlineData(178, 88, "CJ178")]
    public void CoordinatesToCellReference_Returns_CorrectCellReference(int row, int column, string expectedCellReference)
    {
        // Act
        var cellReference = CellReferenceConverter.CoordinatesToCellReference(row, column);

        // Assert
        Assert.Equal(expectedCellReference, cellReference);
    }

    [Theory]
    [InlineData("A1", 1, 1)]
    [InlineData("B2", 2, 2)]
    [InlineData("AA1", 1, 27)]
    [InlineData("CJ178", 178, 88)]
    public void CellReferenceToCoordinates_Returns_CorrectCoordinates(string cellReference, int expectedRow, int expectedColumn)
    {
        // Act
        var (row, column) = CellReferenceConverter.CellReferenceToCoordinates(cellReference);

        // Assert
        Assert.Equal(expectedRow, row);
        Assert.Equal(expectedColumn, column);
    }
}