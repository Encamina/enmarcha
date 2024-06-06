#pragma warning disable S2360 // Optional parameters should not be used

using System.Linq.Expressions;

using Encamina.Enmarcha.Data.Abstractions;

namespace Encamina.Enmarcha.Data.Cosmos;

/// <summary>
/// Represents a repository pattern adapted for Azure Cosmos DB.
/// </summary>
/// <typeparam name="T">The type of (data) entity handled by this read repository.</typeparam>
public interface ICosmosRepository<T> : IAsyncRepository<T>
{
    /// <summary>
    /// Upserts an entity as an asynchronous operation in the Azure Cosmos DB.
    /// </summary>
    /// <param name="entity">The entity to upsert.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>The upserted entity contained within a <see cref="Task"/> object representing the service response for the asynchronous operation.</returns>
    Task<T> AddOrUpdateAsync(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Upserts entities as an asynchronous bulk operation in the Azure Cosmos DB.
    /// </summary>
    /// <param name="entities">Entities to upsert in bulk.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A task that represents the work queued to execute.</returns>
    Task AddOrUpdateBulkAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

    /// <summary>
    /// Counts the number of entities.
    /// </summary>
    /// <returns>The number of entities.</returns>
    int CountAll();

    /// <summary>
    /// Counts the number of entities that satisfy the given predicate.
    /// </summary>
    /// <param name="predicate">A predicate to evaluate on each entity.</param>
    /// <returns>The number of entities that satisfy the given predicate.</returns>
    int CountAll(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Deletes an entity given its unique identifier and partition key as an asynchronous operation.
    /// </summary>
    /// <param name="entityId">The unique identifier of the entity to delete.</param>
    /// <param name="partitionKey">The partition key of the entity to delete.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// A value indicating if the entity was deleted or not contained within a <see cref="Task"/> object representing the service response for the asynchronous operation.
    /// </returns>
    Task<bool> DeleteAsync(string entityId, string partitionKey, CancellationToken cancellationToken);

    /// <summary>
    /// Executes a stored procedure against a container as an asynchronous operation in the Azure Cosmos DB.
    /// </summary>
    /// <param name="storedProcedureId">The identifier of the Stored Procedure to execute.</param>
    /// <param name="partitionKey">The partition key.</param>
    /// <param name="procedureParams">Collection of (dynamic) parameters for the stored procedure.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>The <see cref="Task"/> object representing the service response for the asynchronous operation which would contain any response set in the stored procedure.</returns>
    Task<T> ExecuteStoredProcedureAsync(string storedProcedureId, string partitionKey, dynamic[] procedureParams, CancellationToken cancellationToken);

    /// <summary>
    /// Gets entities from a query for items under a container in an Azure Cosmos DB using sort of a SQL statement.
    /// </summary>
    /// <param name="query">The Azure Cosmos DB SQL query definition.</param>
    /// <param name="skip">Number of items to skip.</param>
    /// <param name="take">Number of items to take.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// A collection of retrieved entities contained within a <see cref="Task"/> object representing the service response for the asynchronous operation.
    /// </returns>
    Task<IQueryable<T>> GetAsync(string query, int skip, int take, CancellationToken cancellationToken);

    /// <summary>
    /// Gets entities from a query for items under a container in an Azure Cosmos DB using sort of a SQL statement.
    /// </summary>
    /// <param name="query">The Azure Cosmos DB SQL query definition.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// A collection of retrieved entities contained within a <see cref="Task"/> object representing the service response for the asynchronous operation.
    /// </returns>
    Task<IQueryable<T>> GetAsync(string query, CancellationToken cancellationToken);

    /// <summary>
    /// Gets all entities from an Azure Cosmos DB container.
    /// </summary>
    /// <returns>
    /// A collection of all entities from an Azure Cosmos DB container.
    /// </returns>
    IQueryable<T> GetAll();

    /// <summary>
    /// Gets all entities from an Azure Cosmos DB container that satisfies a given predicate.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate against each entity in the container.</param>
    /// <returns>
    /// A collection of all entities from an Azure Cosmos DB container that satisfies a given predicate.
    /// </returns>
    IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Gets all ordered entities from an Azure Cosmos DB container that satisfies a given predicate.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate against each entity in the container.</param>
    /// <param name="order">An expression that indicates how to order the returned collection entities.</param>
    /// <param name="descending">A value indicating whether the order should be descending or not (i.e., ascending).</param>
    /// <returns>
    /// A collection of all entities from an Azure Cosmos DB container that satisfies a given predicate, and ordered by the expression.
    /// </returns>
    IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, Expression<Func<T, dynamic>> order, bool descending);

    /// <summary>
    /// Gets ordered entities from an Azure Cosmos DB container that satisfies a given predicate.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate against each entity in the container.</param>
    /// <param name="order">An expression that indicates how to order the returned collection entities.</param>
    /// <param name="descending">A value indicating whether the order should be descending or not (i.e., ascending).</param>
    /// <param name="top">Maximum number of entities to retrieve.</param>
    /// <returns>
    /// A collection of all entities from an Azure Cosmos DB container that satisfies a given predicate, and ordered by the expression.
    /// </returns>
    IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, Expression<Func<T, dynamic>> order, bool descending, int top);

    /// <summary>
    /// Gets ordered entities from an Azure Cosmos DB container that satisfies a given predicate.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate against each entity in the container.</param>
    /// <param name="order">An expression that indicates how to order the returned collection entities.</param>
    /// <param name="descending">A value indicating whether the order should be descending or not (i.e., ascending).</param>
    /// <param name="skip">Number of items to skip.</param>
    /// <param name="take">Number of items to take.</param>
    /// <returns>
    /// A collection of all entities from an Azure Cosmos DB container that satisfies a given predicate, and ordered by the expression.
    /// </returns>
    IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, Expression<Func<T, dynamic>> order, bool descending, int skip, int take);

    /// <summary>
    /// Gets an entity by its unique identifier as an asynchronous operation.
    /// </summary>
    /// <param name="entityId">The unique identifier of the entity to retrieve.</param>
    /// <param name="partitionKey">The partition key of the entity to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>The retrieved entity contained within a <see cref="Task"/> object representing the service response for the asynchronous operation.</returns>
    Task<T?> GetByIdAsync(string entityId, string? partitionKey, CancellationToken cancellationToken);

    /// <summary>
    /// Read all items (entities) from a given partition key.
    /// </summary>
    /// <param name="partitionKey">The partition key.</param>
    /// <param name="maxItemCount">Maximum number of items to read. Defaults to '5'.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of read items (entities).</returns>
    IAsyncEnumerable<T> ReadAllItemsFromPartitionKey(string partitionKey, int maxItemCount = 5);

    /// <summary>
    /// Read all items (entities) from a given query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="maxItemCount">Maximum number of items to read. Defaults to '5'.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> of read items (entities).</returns>
    IAsyncEnumerable<T> ReadAllItemsFromQuery(string query, int maxItemCount = 5);

    /// <summary>
    /// Asynchronously filters items (entities) from a partition key in Azure Cosmos DB.
    /// </summary>
    /// <param name="pred">A predicate to evaluate against each item (entity).</param>
    /// <param name="maxRecords">Maximum number of items (entities) to be returned in the enumeration operation in Azure Cosmos DB.</param>
    /// <param name="partitionKey">The partition key for the Azure Cosmos DB.</param>
    /// <param name="continuationToken">A continuation token in Azure Cosmos DB.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// A tuple containing the filtered items (entities) and a continuation token, contained within a <see cref="Task"/> object representing the service response for the asynchronous operation.
    /// </returns>
    Task<(IQueryable<T> Results, string ContinuationToken)> WhereAsync(Expression<Func<T, bool>> pred, int maxRecords = 0, string partitionKey = "", string continuationToken = "", CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously filters items (entities) from a partition key in Azure Cosmos DB.
    /// </summary>
    /// <param name="query">A query to evaluate against each item (entity).</param>
    /// <param name="maxRecords">Maximum number of items (entities) to be returned in the enumeration operation in Azure Cosmos DB.</param>
    /// <param name="partitionKey">The partition key for the Azure Cosmos DB.</param>
    /// <param name="continuationToken">A continuation token in Azure Cosmos DB.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// A tuple containing the filtered items (entities) and a continuation token, contained within a <see cref="Task"/> object representing the service response for the asynchronous operation.
    /// </returns>
    Task<(IQueryable<T> Results, string ContinuationToken)> WhereAsync(string query, int maxRecords = 0, string partitionKey = "", string continuationToken = "", CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously filters items (entities) from a partition key in Azure Cosmos DB.
    /// </summary>
    /// <typeparam name="TEntity">The specific type of the (data) entities to filter.</typeparam>
    /// <param name="query">A query to evaluate against each item (entity).</param>
    /// <param name="maxRecords">Maximum number of items (entities) to be returned in the enumeration operation in Azure Cosmos DB.</param>
    /// <param name="partitionKey">The partition key for the Azure Cosmos DB.</param>
    /// <param name="continuationToken">A continuation token in Azure Cosmos DB.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// A tuple containing the filtered items (entities) and a continuation token, contained within a <see cref="Task"/> object representing the service response for the asynchronous operation.
    /// </returns>
    Task<(IQueryable<TEntity> Results, string ContinuationToken)> WhereAsync<TEntity>(string query, int maxRecords = 0, string partitionKey = "", string continuationToken = "", CancellationToken cancellationToken = default);
}

#pragma warning restore S2360 // Optional parameters should not be used
