using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Represents an entity that can be validated (by itself).
/// </summary>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class ValidableEntity : IValidableEntity
{
    /// <inheritdoc/>
    public virtual IEnumerable<string> Validate()
    {
        var validationContext = new ValidationContext(this);
        var validationResults = new List<ValidationResult>();
        return !Validator.TryValidateObject(this, validationContext, validationResults, true)
            ? validationResults.Select(validationResult => validationResult.ErrorMessage)
                .Where(errorMessage => errorMessage != null)
                .Select(errorMessage => errorMessage!)
            : Enumerable.Empty<string>();
    }

    /// <inheritdoc/>
    /// <exception cref="AggregateException">
    /// Thrown if the entity is not valid. Inside this exception type, various <see cref="ValidationException"/> are provided with specific validation error messages.
    /// </exception>
    public virtual void ValidateAndThrowException()
    {
        var validationErrorMessages = Validate();

        if (validationErrorMessages.Any())
        {
            throw new AggregateException("One or more validation errors have occurred!", validationErrorMessages.Select(validationErrorMessage => new ValidationException(validationErrorMessage)));
        }
    }
}
