using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Core.DataAnnotations.Resources;
using Encamina.Enmarcha.Core.Extensions;

namespace Encamina.Enmarcha.Core.DataAnnotations;

/// <summary>
/// Specifies that the decorated property is required if a specified condition is met.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class RequiredIfAttribute : ValidationAttribute
{
    private readonly bool allowEmpty;
    private readonly string[] propertyNames;
    private readonly object[] expectedValues;
    private readonly bool failOnAnyCondition;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class.
    /// </summary>
    /// <param name="conditionalPropertyName">The name of the property that represents the condition.</param>
    /// <param name="conditionalValue">The value that triggers the requirement when the condition is met.</param>
    /// <param name="allowEmpty">Determines whether the decorated property, if it is a string, can be empty (i.e., <see cref="string.Empty"/> when the condition is met.</param>
    /// <param name="failOnAnyCondition">Determines whether the validation should fail if any of the conditions are met.</param>
    public RequiredIfAttribute(string conditionalPropertyName, object conditionalValue, bool allowEmpty = false, bool failOnAnyCondition = true)
        : this([conditionalPropertyName], [conditionalValue], allowEmpty, failOnAnyCondition)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class.
    /// </summary>
    /// <param name="conditionalPropertyNames">The names of the properties that represent the conditions.</param>
    /// <param name="conditionalValues">The values that trigger the requirement when the conditions are met.</param>
    /// <param name="allowEmpty">Determines whether the decorated property, if it is a string, can be empty (i.e., <see cref="string.Empty"/> when the condition is met.</param>
    /// <param name="failOnAnyCondition">Determines whether the validation should fail if any of the conditions are met.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the number of items in <paramref name="conditionalPropertyNames"/> and items in <paramref name="conditionalValues"/> do not match.
    /// </exception>
    public RequiredIfAttribute(string[] conditionalPropertyNames, object[] conditionalValues, bool allowEmpty = false, bool failOnAnyCondition = true)
    {
        if (conditionalPropertyNames.Length != conditionalValues.Length)
        {
            throw new ArgumentException(ValudationResultMessages.ResourceManager.GetFormattedStringByCurrentCulture(nameof(ValudationResultMessages.PropertiesConditionsMissmatchRequiredIf)));
        }

        propertyNames = conditionalPropertyNames;
        expectedValues = conditionalValues;

        this.allowEmpty = allowEmpty;
        this.failOnAnyCondition = failOnAnyCondition;
    }

    /// <inheritdoc/>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var objInstance = validationContext.ObjectInstance;
        var objType = objInstance.GetType();

        var validationMessages = new List<string>();

        for (var i = 0; i < propertyNames.Length; i++)
        {
            var propertyName = propertyNames[i];
            var expectedValue = expectedValues[i];

            var propertyInfo = objType.GetProperty(propertyName);

            if (propertyInfo == null)
            {
                return new ValidationResult(ValudationResultMessages.ResourceManager.GetFormattedStringByCurrentCulture(nameof(ValudationResultMessages.MissingPropertyRequiredIf), propertyName));
            }

            var propertyValue = propertyInfo.GetValue(objInstance);

            if (Equals(propertyValue, expectedValue) && (value == null || (value is string s && !allowEmpty && string.IsNullOrWhiteSpace(s))))
            {
                validationMessages.Add(ValudationResultMessages.ResourceManager.GetFormattedStringByCurrentCulture(nameof(ValudationResultMessages.ValueIsRequiredIf), validationContext.DisplayName, propertyName, expectedValue));

                if (failOnAnyCondition)
                {
                    // If `failOnAnyCondition` is `true`, return the first validation message found.
                    return new ValidationResult(validationMessages.Single());
                }
            }
        }

        // Return validation messages if all conditions failed.
        return validationMessages.Count == propertyNames.Length
            ? new ValidationResult(string.Join(Environment.NewLine, validationMessages))
            : ValidationResult.Success;
    }
}
