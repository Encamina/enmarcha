using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Core.Extensions;

using ValidationResultMessages = Encamina.Enmarcha.Core.DataAnnotations.Resources.ValudationResultMessages;

namespace Encamina.Enmarcha.Core.DataAnnotations;

/// <summary>
/// Provides a more specific validation for <see cref="string"/> types, in this case just checking it is not empty or whitespace.
/// A <see langword="null"/> value is considered valid.
/// </summary>
/// <remarks>
/// This is attribute complements <see cref="RequiredAttribute"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class NotEmptyOrWhitespaceAttribute : ValidationAttribute
{
    /// <inheritdoc/>
    public override bool IsValid(object? value) => value == null || (value is string s && !string.IsNullOrWhiteSpace(s));

    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return IsValid(value)
            ? ValidationResult.Success
            : new ValidationResult(ValidationResultMessages.ResourceManager.GetFormattedStringByCurrentCulture(nameof(ValidationResultMessages.ValueIsEmptyOrWhiteSpace), validationContext.MemberName));
    }
}
