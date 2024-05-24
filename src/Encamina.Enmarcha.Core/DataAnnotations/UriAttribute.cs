using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Core.Extensions;

using ValidationResultMessages = Encamina.Enmarcha.Core.DataAnnotations.Resources.ValudationResultMessages;

namespace Encamina.Enmarcha.Core.DataAnnotations;

/// <summary>
/// Provides <see cref="Uri"/> validation.
/// </summary>
/// <remarks>
/// This is an alternative to <see cref="UrlAttribute"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class UriAttribute : DataTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UriAttribute"/> class.
    /// </summary>
    public UriAttribute() : base(DataType.Url)
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether <see langword="null"/> is allowed.
    /// </summary>
    public bool AllowsNull { get; set; } = true;

    /// <inheritdoc/>
    public override bool IsValid(object? value) => (AllowsNull && value == null) || (value is Uri uri && uri.IsWellFormedOriginalString());

    /// <inheritdoc/>
    /// <remarks>
    /// A null value is accepted as valid since the idea of «Not Empty» doesn't necessarily means «Required».
    /// </remarks>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return IsValid(value)
            ? ValidationResult.Success
            : new ValidationResult(ValidationResultMessages.ResourceManager.GetFormattedStringByCurrentCulture(nameof(ValidationResultMessages.ValuieIsInvalidUri), validationContext.MemberName));
    }
}
