namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Models.Excel;

/// <summary>
/// Represents configuration for loading an Excel document.
/// </summary>
public class ExcelLoadOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether empty rows should be excluded.
    /// </summary>
    public bool ExcludeEmptyRows { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether empty columns should be excluded.
    /// </summary>
    public bool ExcludeEmptyColumns { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether hidden rows should be excluded.
    /// </summary>
    public bool ExcludeHiddenRows { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether hidden columns should be excluded.
    /// </summary>
    public bool ExcludeHiddenColumns { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether hidden worksheets should be excluded.
    /// </summary>
    public bool ExcludeHiddenSheets { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to load only the cells range with text.
    /// Default is false.
    /// For example, if the range is A1:C3, but only A1, B1, C1, A2 contains text, and this property is set to true, only the range A1:C2 is loaded.
    /// </summary>
    public bool LoadOnlyCellsRangeWithText { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the rules for merging empty columns.
    /// </summary>
    public MergeEmptyElementsRules MergeEmptyColumnsRules { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the rules for merging empty rows.
    /// </summary>
    public MergeEmptyElementsRules MergeEmptyRowsRules { get; set; }
}