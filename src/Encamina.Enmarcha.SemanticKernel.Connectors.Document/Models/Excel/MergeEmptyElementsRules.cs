namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Models.Excel;

/// <summary>
/// Represents the rules for merging empty elements.
/// </summary>
public class MergeEmptyElementsRules
{
    /// <summary>
    /// Gets or sets a value indicating the minimum number of elements to merge.
    /// </summary>
    public int MinimumElementsToMerge { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the resulting number of elements from the merge.
    /// </summary>
    public int ResultingElementsFromMerge { get; set; }
}