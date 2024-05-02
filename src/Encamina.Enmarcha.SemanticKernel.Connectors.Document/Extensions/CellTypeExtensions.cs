using System.Globalization;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using ExcelNumberFormat;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="CellType"/> class.
/// </summary>
internal static class CellTypeExtensions
{
    private static readonly Dictionary<int, string> DefaultFormatCodes = new()
    {
        { 0, string.Empty },
        { 1, "0" },
        { 2, "0.00" },
        { 3, "#,##0" },
        { 4, "#,##0.00" },
        { 7, "$#,##0.00_);($#,##0.00)" },
        { 9, "0%" },
        { 10, "0.00%" },
        { 11, "0.00E+00" },
        { 12, "# ?/?" },
        { 13, "# ??/??" },
        { 14, "M/d/yyyy" },
        { 15, "d-MMM-yy" },
        { 16, "d-MMM" },
        { 17, "MMM-yy" },
        { 18, "h:mm tt" },
        { 19, "h:mm:ss tt" },
        { 20, "H:mm" },
        { 21, "H:mm:ss" },
        { 22, "M/d/yyyy H:mm" },
        { 37, "#,##0 ;(#,##0)" },
        { 38, "#,##0 ;[Red](#,##0)" },
        { 39, "#,##0.00;(#,##0.00)" },
        { 40, "#,##0.00;[Red](#,##0.00)" },
        { 45, "mm:ss" },
        { 46, "[h]:mm:ss" },
        { 47, "mm:ss.0" },
        { 48, "##0.0E+0" },
        { 49, "@" },
    };

    /// <summary>
    /// Gets the value of the cell and a boolean indicating whether the cell is text.
    /// </summary>
    /// <param name="cell">The cell to get the value from.</param>
    /// <param name="workbookPart">The workbook part that contains the cell.</param>
    /// <returns>A tuple containing the cell value and a boolean indicating whether the cell contains text.</returns>
    public static (string Value, bool IsText) GetCellValue(this CellType cell, WorkbookPart workbookPart)
    {
        if (cell == null)
        {
            return (null, false);
        }

        if (cell.DataType?.Value == CellValues.SharedString && int.TryParse(cell.InnerText, out var index) && workbookPart.SharedStringTablePart?.SharedStringTable != null)
        {
            var openXmlElement = workbookPart.SharedStringTablePart.SharedStringTable.ElementAtOrDefault(index);
            var stringValue = openXmlElement != null
                ? openXmlElement.InnerText
                : cell.CellValue?.InnerText;

            return (stringValue, true);
        }

        return (cell.CellValue?.InnerText, false);
    }

    /// <summary>
    /// Gets the raw text value of the cell.
    /// </summary>
    /// <param name="cell">The cell to get the value from.</param>
    /// <param name="workbookPart">The workbook part that contains the cell.</param>
    /// <returns>The raw text value of the cell.</returns>
    public static string? GetCellTextValue(this CellType cell, WorkbookPart workbookPart)
    {
        if (cell == null)
        {
            return null;
        }

        if (cell.DataType?.Value == CellValues.SharedString && int.TryParse(cell.InnerText, out var index) && workbookPart.SharedStringTablePart?.SharedStringTable != null)
        {
            var openXmlElement = workbookPart.SharedStringTablePart.SharedStringTable.ElementAtOrDefault(index);
            var stringValue = openXmlElement != null
                ? openXmlElement.InnerText
                : cell.CellValue?.InnerText;

            return stringValue;
        }

        return cell.CellValue?.InnerText;
    }

    /// <summary>
    /// Gets the formatted text value of the cell.
    /// </summary>
    /// <param name="cell">The cell to get the value from.</param>
    /// <param name="workbookPart">The workbook part that contains the cell.</param>
    /// <returns>The formatted text value of the cell.</returns>
    public static string? GetCellFormattedTextValue(this CellType cell, WorkbookPart workbookPart)
    {
        if (cell == null)
        {
            return null;
        }

        if ((cell.DataType == null || cell.DataType.Value == CellValues.Number)
            && double.TryParse(cell.CellValue?.InnerText, out var number)
            && cell.StyleIndex != null && workbookPart.WorkbookStylesPart?.Stylesheet is { CellFormats: not null, NumberingFormats: not null })
        {
            var cellFormat = workbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ElementAtOrDefault((int)cell.StyleIndex.Value) as CellFormat;

            if (cellFormat?.NumberFormatId != null)
            {
                string? format = workbookPart.WorkbookStylesPart.Stylesheet.NumberingFormats.Elements<NumberingFormat>()
                    .FirstOrDefault(i => i.NumberFormatId == cellFormat.NumberFormatId)?
                    .FormatCode;

                if (format == null && DefaultFormatCodes.TryGetValue((int)cellFormat.NumberFormatId.Value, out var defaultFormat))
                {
                    format = defaultFormat;
                }

                return ConvertToExcelFormat(number, format);
            }
        }

        return GetCellTextValue(cell, workbookPart);
    }

    /// <summary>
    /// Gets the raw text value of the cell.
    /// </summary>
    /// <param name="cell">The cell to get the value from.</param>
    /// <param name="workbookPart">The workbook part that contains the cell.</param>
    /// <returns>The raw text value of the cell.</returns>
    public static (bool IsBold, bool IsItalic) GetCellFont(this CellType cell, WorkbookPart workbookPart)
    {
        if (cell?.StyleIndex == null || workbookPart.WorkbookStylesPart?.Stylesheet.CellFormats == null || workbookPart.WorkbookStylesPart?.Stylesheet.Fonts == null)
        {
            return (false, false);
        }

        var cellFormat = workbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ElementAtOrDefault((int)cell.StyleIndex.Value) as CellFormat;
        if (cellFormat?.FontId == null)
        {
            return (false, false);
        }

        var font = workbookPart.WorkbookStylesPart.Stylesheet.Fonts.Elements<Font>().ElementAtOrDefault((int)cellFormat.FontId.Value);
        if (font == null)
        {
            return (false, false);
        }

        return (font.Bold != null, font.Italic != null);
    }

    /// <summary>
    /// Converts the specified value to an Excel format.
    /// </summary>
    /// <param name="value">The object to format.</param>
    /// <param name="format">The format to apply.</param>
    /// <returns>A string representation of the object in the specified Excel format.</returns>
    private static string ConvertToExcelFormat(object value, string format)
    {
        var numberFormat = new NumberFormat(format);

        return numberFormat.IsValid
            ? numberFormat.Format(value, CultureInfo.InvariantCulture)
            : format;
    }
}
