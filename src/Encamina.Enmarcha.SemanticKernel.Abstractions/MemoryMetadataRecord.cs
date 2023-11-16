using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Core.DataAnnotations;
using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.SemanticKernel.Memory;

namespace Encamina.Enmarcha.SemanticKernel.Abstractions;

/// <summary>
/// Represents a generic record that can be stored as additional metadata in a memory record.
/// </summary>
/// <seealso cref="MemoryRecordMetadata.AdditionalMetadata"/>
public class MemoryMetadataRecord : IdentifiableBase<string>
{
    /// <inheritdoc/>
    [Required]
    [NotEmptyOrWhitespace]
    public override string Id
    {
        get => base.Id;
        init => base.Id = value;
    }

    /// <summary>
    /// Gets the name of the collection that this memory metadata record belongs to.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string CollectionName { get; init; }

    /// <summary>
    /// Gets the number of chunks. This metadata is usually valuable to now from
    /// one memory instance if it belongs to a memory with multiple chunks.
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public int ChunksNumber { get; init; }

    /// <summary>
    /// Gets the label of the metadata, which identifies the metadata value.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string MetadataLabel { get; init; }

    /// <summary>
    /// Gets the value of the metadata.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string MetadataValue { get; init; }

    /// <summary>
    /// Gets the UTC date and time when this memory metadata record was updated (created or updated).
    /// </summary>
    public DateTimeOffset LastUpdateUtc { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets a value indicating whether this memory metadata record is active or not.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Activates this memory metadata record. This action updates the properties <see cref="IsActive"/>
    /// to <see langword="true"/> and <see cref="LastUpdateUtc"/> to UTC now.
    /// </summary>
    public void Activate() => SetActive(true);

    /// <summary>
    /// Deactivates this memory metadata record. This action updates the properties <see cref="IsActive"/>
    /// to <see langword="false"/> and <see cref="LastUpdateUtc"/> to UTC now.
    /// </summary>
    public void Deactivate() => SetActive(false);

    private void SetActive(bool isActive)
    {
        IsActive = isActive;
        LastUpdateUtc = DateTimeOffset.UtcNow;
    }
}
