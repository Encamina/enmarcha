namespace Encamina.Enmarcha.Data.Abstractions;

/// <summary>
/// Represents a unit of work that can manage both synchronous and asynchronous operations.
/// </summary>
/// <remarks>
/// A unit of work is referred to as a single transaction that involves multiple get, insert, update, or delete operations.
/// In simple terms, it means that for a specific action (such as a registration on a website for eqxmple), all the get, insert,
/// update, and delete operations are handled as a single transaction. This is more efficient than handling multiple transactions in a chattier way.
/// </remarks>
public interface IFullUnitOfWork : IUnitOfWork, IAsyncUnitOfWork
{
}
