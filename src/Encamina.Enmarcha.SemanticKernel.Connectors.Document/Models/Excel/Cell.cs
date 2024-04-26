using DocumentFormat.OpenXml.Packaging;

using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Extensions;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Models.Excel;

/// <summary>
/// Represents a cell in an Excel worksheet.
/// </summary>
internal class Cell
{
    /// <summary>
    /// Gets the text in the cell.
    /// </summary>
    public string Text { get; private init; }

    /// <summary>
    /// Gets the formatted text in the cell.
    /// </summary>
    public string FormattedText { get; private init; }

    /// <summary>
    /// Gets a value indicating whether the text in the cell is null, empty or white space.
    /// </summary>
    public bool IsNullOrWhiteSpace { get; private init; }

    /// <summary>
    /// Gets the reference of the cell. Such us A1, B2, etc.
    /// </summary>
    public string Reference { get; private init; }

    /// <summary>
    /// Gets a value indicating whether the text in the cell is bold.
    /// </summary>
    public bool IsBold { get; private init; }

    /// <summary>
    /// Gets a value indicating whether the text in the cell is italic.
    /// </summary>
    public bool IsItalic { get; private init; }

    /// <summary>
    /// Creates a Cell object from a <see cref="DocumentFormat.OpenXml.Spreadsheet.Cell"/> object and a <see cref="WorkbookPart"/> object.
    /// </summary>
    /// <param name="cell">The <see cref="DocumentFormat.OpenXml.Spreadsheet.Cell"/> object representing the cell in the Excel worksheet.</param>
    /// <param name="workbookPart">The <see cref="WorkbookPart"/> object containing the <see cref="DocumentFormat.OpenXml.Spreadsheet.Cell"/> object.</param>
    /// <returns>A Cell object representing the cell in the Excel worksheet.</returns>
    public static Cell Create(DocumentFormat.OpenXml.Spreadsheet.Cell cell, WorkbookPart workbookPart)
    {
        var cellFont = cell.GetCellFont(workbookPart);
        var text = cell.GetCellTextValue(workbookPart);
        var formattedText = cell.GetCellFormattedTextValue(workbookPart);

        return new Cell
        {
            Text = text,
            FormattedText = formattedText,
            IsNullOrWhiteSpace = string.IsNullOrWhiteSpace(text) && string.IsNullOrWhiteSpace(formattedText),
            Reference = cell.CellReference,
            IsBold = cellFont.IsBold,
            IsItalic = cellFont.IsItalic,
        };
    }

    /// <summary>
    /// Creates an empty Cell object with the specified reference.
    /// </summary>
    /// <param name="reference">The reference of the empty cell.</param>
    /// <returns>An empty Cell object with the specified reference.</returns>
    public static Cell Empty(string reference)
    {
        return new Cell
        {
            IsNullOrWhiteSpace = true,
            Reference = reference,
        };
    }
}
