namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Models.Excel;

/// <summary>
/// Represents configuration for loading an Excel document.
/// </summary>
public class ExcelLoadOptions
{
    /// <summary>
    /// Specifies whether empty rows should be excluded.
    /// Default is false.
    /// </summary>
    public bool ExcludeEmptyRows { get; set; }

    /// <summary>
    /// Specifies whether empty columns should be excluded.
    /// Default is false.
    /// </summary>
    public bool ExcludeEmptyColumns { get; set; }

    /// <summary>
    /// Specifies whether to load hidden sheets or not.
    /// Default is true.
    /// </summary>
    public bool LoadHiddenSheets { get; set; } = true;

    /// <summary>
    /// Specifies whether to load only the cells range with text.
    /// Default is false.
    /// For example, if the range is A1:C3, but only A1, B1, C1, A2 contains text, and this property is set to true, only the range A1:C2 is loaded.
    /// </summary>
    public bool LoadOnlyCellsRangeWithText { get; set; }
}