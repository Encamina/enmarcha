using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Core.DataAnnotations.Resources;
using Encamina.Enmarcha.Core.Extensions;

namespace Encamina.Enmarcha.Core.DataAnnotations;

/// <summary>
/// Specifies that the decorated property is required if a specified condition is met.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredIfAttribute : ValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class.
    /// </summary>
    /// <param name="conditionalPropertyName">The name of the property that represents the condition.</param>
    /// <param name="conditionalValue">The value that triggers the requirement when the condition is met.</param>
    public RequiredIfAttribute(string conditionalPropertyName, object conditionalValue)
    {
        ConditionalPropertyName = conditionalPropertyName;
        ConditionalValue = conditionalValue;
    }

    /// <summary>
    /// Gets the name of the property that represents the condition.
    /// </summary>
    public string ConditionalPropertyName { get; }

    /// <summary>
    /// Gets the value that triggers the requirement when the condition is met.
    /// </summary>
    public object ConditionalValue { get; }

    /// <inheritdoc/>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var conditionalValue = validationContext.ObjectInstance
            .GetType()
            .GetProperty(ConditionalPropertyName)?
            .GetValue(validationContext.ObjectInstance, null);

        return Equals(ConditionalValue, conditionalValue) && value == null
            ? new ValidationResult(ValudationResultMessages.ResourceManager.GetFormattedStringByCurrentCulture(nameof(ValudationResultMessages.ValueIsRequiredIf), validationContext.DisplayName, ConditionalPropertyName, ConditionalValue))
            : ValidationResult.Success;
    }
}
