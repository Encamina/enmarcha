using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.Services.Abstractions;

/// <summary>
/// Represents a basic execution context.
/// </summary>
public interface IExecutionContext : IExecutionContextTemplate, IIdentifiable<Guid>
{
}
