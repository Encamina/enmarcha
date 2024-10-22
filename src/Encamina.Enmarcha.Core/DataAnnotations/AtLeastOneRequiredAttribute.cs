using System.ComponentModel.DataAnnotations;
using System.Reflection;

using Encamina.Enmarcha.Core.Extensions;

using ValidationResultMessages = Encamina.Enmarcha.Core.DataAnnotations.Resources.ValudationResultMessages;

namespace Encamina.Enmarcha.Core.DataAnnotations;

/// <summary>
/// Attribute that enforces validation on a class by requiring at least one of the specified properties to have a non-null or non-empty value.
/// </summary>
/// <remarks>
/// This attribute is typically used on models to ensure that at least one of the specified properties is populated.
/// If none of the specified properties have a value, the validation will fail.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AtLeastOneRequiredAttribute : ValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AtLeastOneRequiredAttribute"/> class with the specified property names.
    /// </summary>
    /// <param name="propertyNames">An array of property names that must be validated.</param>
    public AtLeastOneRequiredAttribute(params string[] propertyNames)
    {
        PropertyNames = propertyNames;
    }

    /// <summary>
    /// Gets the array of property names that must be validated to ensure at least one has a value.
    /// </summary>
    public string[] PropertyNames { get; }

    /// <summary>
    /// Validates that at least one of the specified properties has a non-null or non-empty value.
    /// </summary>
    /// <param name="value">The object to validate, typically the class instance this attribute is applied to.</param>
    /// <param name="validationContext">Provides contextual information about the validation operation.</param>
    /// <returns>
    /// <see cref="ValidationResult.Success"/> if at least one property has a value; otherwise, a <see cref="ValidationResult"/> indicating the error.
    /// </returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var type = value.GetType();
        var atLeastOneHasValue = false;

        foreach (var propertyName in PropertyNames)
        {
            var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                return new ValidationResult(ValidationResultMessages.ResourceManager.GetFormattedStringByCurrentCulture(nameof(ValidationResultMessages.PropertyNotFound), propertyName));
            }

            var propertyValue = property.GetValue(value);
            if (propertyValue is string str)
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    atLeastOneHasValue = true;
                    break;
                }
            }
            else if (propertyValue != null)
            {
                atLeastOneHasValue = true;
                break;
            }
        }

        if (!atLeastOneHasValue)
        {
            var propertyNames = string.Join(", ", PropertyNames);
            return new ValidationResult(ValidationResultMessages.ResourceManager.GetFormattedStringByCurrentCulture(nameof(ValidationResultMessages.MissingRequiredProperty), propertyNames));
        }

        return ValidationResult.Success;
    }
}
