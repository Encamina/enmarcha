﻿using System.ComponentModel.DataAnnotations;

namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Options for semantic text splitters.
/// </summary>
public class SemanticTextSplitterOptions
{
    /// <summary>
    /// Gets size of the buffer used in semantic text splitting. It represents the number of sentences to include on each side of the current sentence within the buffer.
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public int BufferSize { get; init; } = 1;

    /// <summary>
    /// Gets type of threshold used for identifying breakpoints in the text. It can be based on percentiles, standard deviations, or interquartile range.
    /// </summary>
    [Required]
    public BreakpointThresholdType BreakpointThresholdType { get; init; } = BreakpointThresholdType.Percentile;

    /// <summary>
    /// Gets amount used in the threshold calculation for identifying breakpoints. The interpretation depends on the selected threshold type.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///     <item>
    ///         For BreakpointThresholdType.Percentile, a valid value is 95.
    ///     </item>
    ///     <item>
    ///         For BreakpointThresholdType.StandardDeviation, a valid value is 3.
    ///     </item>
    ///     <item>
    ///         For BreakpointThresholdType.Interquartile, a valid value is 1.5.
    ///     </item>
    /// </list>
    /// </remarks>
    [Required]
    public float BreakpointThresholdAmount { get; init; } = 95;

    /// <summary>
    /// Gets maximum allowed size for each chunk. If specified, the text will be split into chunks with a maximum size as defined by this property.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int? MaxChunkSize { get; init; }

    /// <summary>
    /// Gets maximum number of attempts to split a chunk if its length exceeds the defined maximum chunk size.
    /// If specified, the splitter will make multiple attempts to split the chunk while respecting the size limit.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int? ChunkSplitRetryLimit { get; init; }
}
