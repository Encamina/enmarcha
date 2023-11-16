using System.ComponentModel.DataAnnotations;

namespace Encamina.Enmarcha.Core.DataAnnotations;

/// <summary>
/// Validates that a data field value is required if another property in the same instance is not null.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequireWhenOtherPropertyNotNullAttribute : ValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequireWhenOtherPropertyNotNullAttribute"/> class.
    /// </summary>
    /// <param name="otherPropertyName">The name of other property in the same instance to check for <see langword="null"/>.</param>
    public RequireWhenOtherPropertyNotNullAttribute(string otherPropertyName)
    {
        OtherPropertyName = otherPropertyName;
    }

    /// <summary>
    /// Gets the name of the other property.
    /// </summary>
    public string OtherPropertyName { get; }

    /// <inheritdoc/>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var otherPropertyValue = validationContext.ObjectInstance
                                                  .GetType()
                                                  .GetProperty(OtherPropertyName)
                                                  .GetValue(validationContext.ObjectInstance, null);

        return otherPropertyValue != null && value == null
            ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName))
            : ValidationResult.Success;
    }
}
