using Encamina.Enmarcha.Core.Extensions;

using ExceptionMessages = Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions.Resources.ExceptionMessages;

namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Metadata options to filter questions and answers.
/// </summary>
public class MetadataOptions
{
    /// <summary>
    /// Gets the current collection of metadata values.
    /// </summary>
    public virtual IDictionary<string, string> Metadata { get; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets or sets the logical operation used to join the metadata values from <see cref="Metadata"/>.
    /// </summary>
    public virtual LogicalOperation LogicalOperation { get; set; }

    /// <summary>
    /// Checks if given <paramref name="metadata"/> collection satisfies this metadata options based on its <see cref="LogicalOperation">logical operation</see>.
    /// </summary>
    /// <param name="metadata">The metadata collection to check agains this metadata options.</param>
    /// <returns>
    /// Returns <see langword="true"/> if the given <paramref name="metadata"/> collection checks positively agains this metadata options; otherwise, returns <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If an invalid or unsupported <see cref="LogicalOperation">logical operation</see> is set on this metadata options.
    /// </exception>
    public bool CheckMetadata(IDictionary<string, string> metadata)
    {
        return metadata?.Any() == true && LogicalOperation switch
        {
            LogicalOperation.Or => metadata.Union(Metadata, new MetadataEqualityComparer()).Count() >= Metadata.Count,
            LogicalOperation.And => metadata.Intersect(Metadata, new MetadataEqualityComparer()).Count() == Metadata.Count,
            _ => throw new ArgumentOutOfRangeException(string.Format(ExceptionMessages.ResourceManager.GetStringByCurrentCulture(nameof(ExceptionMessages.InvalidMetadataLogicalOperation)), LogicalOperation)),
        };
    }

    /// <summary>
    /// Merges an instance of <see cref="MetadataOptions"/> into this instance.
    /// </summary>
    /// <remarks>
    /// If duplicate values of metadata exists, this method keeps the value from this instance (instead of throwing an exception, or keeping the value from <paramref name="metadataOptions"/>).
    /// </remarks>
    /// <param name="metadataOptions">
    /// An instance of <see cref="MetadataOptions"/> to merge into this instance. If <see langword="null" /> this method does nothing.
    /// </param>
    public void MergeMetadataOptions(MetadataOptions metadataOptions)
    {
        Metadata.Merge(metadataOptions?.Metadata);
    }

    private sealed class MetadataEqualityComparer : IEqualityComparer<KeyValuePair<string, string>>
    {
        public bool Equals(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
        {
            return x.Key == y.Key && x.Value == y.Value;
        }

        public int GetHashCode(KeyValuePair<string, string> obj)
        {
            return !string.IsNullOrWhiteSpace(obj.Key) ? obj.Key.GetHashCode() : 0;
        }
    }
}
