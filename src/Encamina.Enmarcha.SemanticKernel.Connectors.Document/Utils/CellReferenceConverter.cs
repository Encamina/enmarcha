namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Utils;

/// <summary>
/// Class that provides methods to convert between row and column coordinates and cell references in spreadsheet format.
/// </summary>
public static class CellReferenceConverter
{
    /// <summary>
    /// Converts row and column coordinates to a cell reference in spreadsheet format.
    /// </summary>
    /// <param name="row">Row number.</param>
    /// <param name="column">Column number.</param>
    /// <returns>Cell reference in spreadsheet format (e.g., "A1", "B2", etc.).</returns>
    public static string CoordinatesToCellReference(int row, int column)
    {
        var columnString = string.Empty;
        while (column > 0)
        {
            var remainder = (column - 1) % 26;
            columnString = (char)('A' + remainder) + columnString;
            column = (column - 1) / 26;
        }

        return $"{columnString}{row}";
    }

    /// <summary>
    /// Converts a cell reference to row and column coordinates.
    /// </summary>
    /// <param name="cellReference">Cell reference in spreadsheet format (e.g., "A1", "B2", etc.).</param>
    /// <returns>Row and column coordinates as a tuple.</returns>
    public static (int Row, int Column) CellReferenceToCoordinates(string cellReference)
    {
        var columnString = new string(cellReference.TakeWhile(char.IsLetter).ToArray());
        var rowString = cellReference[columnString.Length..];

        var column = columnString.Aggregate(0, (result, c) => (result * 26) + c - 'A' + 1);
        var row = int.Parse(rowString);

        return (row, column);
    }
}
