namespace Encamina.Enmarcha.Data.Abstractions;

/// <summary>
/// <para>
/// In terms of Domain-Driven Design (DDD), an aggregate is a logical group of entities and value objects that are treated as a single unit. In other words, an entity
/// that is an aggregate root is a single one entity that all others entities are bound to.
/// </para>
/// <para>
/// An empty interface, sometimes called a marker interface, typically used in a 'Command and Query Responsibility Segregation' (CQRS) pattern to simply indicate that
/// an entity class is also an aggregate root (or root entity), representing task-based interfaces where consumers are guided through a complex process as a series of
/// steps, or with complex domain models as a single unit for data changes (hence an aggregate in DDD terminology) and ensure that these entities (objects) are always
/// in a consistent state.
/// </para>
/// </summary>
/// <remarks>
/// A marker interface is sometimes considered as an anti-pattern; however, it is also a clean way to mark a class, especially when that interface
/// might be evolving. An attribute could be the other choice for the marker, but it is quicker to see any base class next to the <c>IAggregateRoot</c>
/// interface instead of putting an <c>AggregateAttribute</c> attribute marker above the class. It is a matter of preferences, in any case.
/// </remarks>
/// <seealso href="https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs"/>
public interface IAggregateRoot
{
}
