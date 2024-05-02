using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Core.Extensions;

using ValidationResultMessages = Encamina.Enmarcha.Core.DataAnnotations.Resources.ValudationResultMessages;

namespace Encamina.Enmarcha.Core.DataAnnotations;

/// <summary>
/// Provides a more specific validation for <see cref="Guid"/> types, checking it is empty.
/// </summary>
/// <remarks>
/// This is attribute complements <see cref="RequiredAttribute"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class GuidNotEmptyAttribute : ValidationAttribute
{
    /// <inheritdoc/>
    /// <remarks>
    /// A null value is accepted as valid since the idea of «Not Empty» doesn't necessarily means «Required».
    /// </remarks>
    public override bool IsValid(object value) => value == null || (value is Guid s && s != Guid.Empty);

    /// <inheritdoc/>
    /// <remarks>
    /// A null value is accepted as valid since the idea of «Not Empty» doesn't necessarily means «Required».
    /// </remarks>
    protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
    {
        return IsValid(value)
            ? ValidationResult.Success
            : new ValidationResult(ValidationResultMessages.ResourceManager.GetFormattedStringByCurrentCulture(nameof(ValidationResultMessages.ValueIsInvalidGuid), validationContext.MemberName));
    }
}
