using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Net;

using Encamina.Enmarcha.Core.Extensions;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

using ExceptionMessages = Encamina.Enmarcha.Data.Cosmos.Resources.ExceptionMessages;

namespace Encamina.Enmarcha.Data.Cosmos;

/// <summary>
/// Internal implementation of <see cref="ICosmosRepository{T}"/>.
/// </summary>
/// <typeparam name="T">The type of (data) entity handled by this read repository.</typeparam>
internal sealed class CosmosRepository<T> : ICosmosRepository<T>
{
    private readonly Container container;

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosRepository{T}"/> class.
    /// </summary>
    /// <param name="initializer">A valid instance of <see cref="ICosmosInitializer"/>.</param>
    /// <param name="container">The name of the container.</param>
    public CosmosRepository(ICosmosInitializer initializer, string container)
    {
        this.container = initializer.GetContainer(container);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosRepository{T}"/> class.
    /// </summary>
    /// <param name="initializer">A valid instance of <see cref="ICosmosInitializer"/>.</param>
    /// <param name="database">The name of the database.</param>
    /// <param name="container">The name of the container.</param>
    public CosmosRepository(ICosmosInitializer initializer, string database, string container)
    {
        this.container = initializer.GetContainer(database, container);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosRepository{T}"/> class.
    /// </summary>
    /// <param name="container">The container.</param>
    public CosmosRepository(Container container)
    {
        this.container = container;
    }

    /// <inheritdoc/>
    public async Task<T> AddOrUpdateAsync(T entity, CancellationToken cancellationToken)
    {
        try
        {
            var upsertedDoc = await container.UpsertItemAsync<T>(entity, cancellationToken: cancellationToken);

            return upsertedDoc.Resource;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ExceptionMessages.ResourceManager.GetFormattedStringByCurrentUICulture(nameof(ExceptionMessages.AddOrUpdateEntityException), entity), ex);
        }
    }

    /// <inheritdoc/>
    public async Task AddOrUpdateBulkAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
    {
        Task aggregationTask = null;

        try
        {
            var bulkTasks = new List<Task>();

            foreach (var entity in entities)
            {
                bulkTasks.Add(container.UpsertItemAsync(entity, cancellationToken: cancellationToken));
            }

            aggregationTask = Task.WhenAll(bulkTasks);
            await aggregationTask;
        }
        catch (Exception ex)
        {
            if (aggregationTask?.Exception != null)
            {
                throw aggregationTask.Exception;
            }

            throw new InvalidOperationException(ExceptionMessages.ResourceManager.GetFormattedStringByCurrentUICulture(nameof(ExceptionMessages.AddOrUpdateBulkException), entities), ex);
        }
    }

    /// <inheritdoc/>
    public int CountAll() => container.GetItemLinqQueryable<T>(true).Count();

    /// <inheritdoc/>
    public int CountAll(Expression<Func<T, bool>> predicate) => container.GetItemLinqQueryable<T>(true).Count(predicate);

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(string entityId, string partitionKey, CancellationToken cancellationToken)
    {
        try
        {
            var partition = GeneratePartition(partitionKey);

            await container.DeleteItemAsync<T>(entityId, partition, cancellationToken: cancellationToken);

            return true;
        }
        catch (CosmosException ce) when (ce.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ExceptionMessages.ResourceManager.GetFormattedStringByCurrentUICulture(nameof(ExceptionMessages.DeleteItemByIdException), entityId, partitionKey), ex);
        }
    }

    /// <inheritdoc/>
    public async Task<T> ExecuteStoredProcedureAsync(string storedProcedureId, string partitionKey, dynamic[] procedureParams, CancellationToken cancellationToken)
    {
        try
        {
            return (await container.Scripts.ExecuteStoredProcedureAsync<T>(storedProcedureId, GeneratePartition(partitionKey), procedureParams, cancellationToken: cancellationToken)).Resource;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ExceptionMessages.ResourceManager.GetFormattedStringByCurrentUICulture(nameof(ExceptionMessages.ExecuteStoredProcedureException), storedProcedureId, partitionKey), ex);
        }
    }

    /// <inheritdoc/>
    public IQueryable<T> GetAll()
    {
        var entities = container.GetItemLinqQueryable<T>(true).ToList(); // Enumerate to retrieve entities...
        return entities.AsQueryable();
    }

    /// <inheritdoc/>
    public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
    {
        var entities = container.GetItemLinqQueryable<T>(true).Where(predicate).ToList(); // Enumerate to retrieve entities...
        return entities.AsQueryable();
    }

    /// <inheritdoc/>
    public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, Expression<Func<T, dynamic>> order, bool descending)
    {
        var query = container.GetItemLinqQueryable<T>(true).Where(predicate);
        var entities = (descending ? query.OrderByDescending(order) : query.OrderBy(order)).ToList(); // Enumerate to retrieve entities...
        return entities.AsQueryable();
    }

    /// <inheritdoc/>
    public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, Expression<Func<T, dynamic>> order, bool descending, int top)
    {
        var query = container.GetItemLinqQueryable<T>(true).Where(predicate);
        var entities = (descending ? query.OrderByDescending(order) : query.OrderBy(order)).Take(top).ToList(); // Enumerate to retrieve entities...
        return entities.AsQueryable();
    }

    /// <inheritdoc/>
    public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, Expression<Func<T, dynamic>> order, bool descending, int skip, int take)
    {
        var query = container.GetItemLinqQueryable<T>(true).Where(predicate);
        var entities = (descending ? query.OrderByDescending(order) : query.OrderBy(order)).Skip(skip).Take(take).ToList(); // Enumerate to retrieve entities...
        return entities.AsQueryable();
    }

    /// <inheritdoc/>
    public async Task<IQueryable<T>> GetAsync(string query, int skip, int take, CancellationToken cancellationToken) => await GetAsync($@"{query} OFFSET {skip} LIMIT {take}", cancellationToken);

    /// <inheritdoc/>
    public async Task<IQueryable<T>> GetAsync(string query, CancellationToken cancellationToken)
    {
        var result = new List<T>();

        var queryResultSetIterator = container.GetItemQueryIterator<T>(new QueryDefinition(query));

        while (queryResultSetIterator.HasMoreResults)
        {
            var currentResultSet = await queryResultSetIterator.ReadNextAsync(cancellationToken);

            foreach (var item in currentResultSet)
            {
                result.Add(item);
            }
        }

        return result.AsQueryable();
    }

    /// <inheritdoc/>
    public async Task<T> GetByIdAsync(string entityId, string partitionKey, CancellationToken cancellationToken)
    {
        try
        {
            return await container.ReadItemAsync<T>(entityId, GeneratePartition(partitionKey), cancellationToken: cancellationToken);
        }
        catch (CosmosException ce) when (ce.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ExceptionMessages.ResourceManager.GetFormattedStringByCurrentUICulture(nameof(ExceptionMessages.GetByIdException), entityId, partitionKey), ex);
        }
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<T> ReadAllItemsFromPartitionKey(string partitionKey, int maxItemCount = 5)
    {
        using var resultSet = container.GetItemQueryIterator<T>(
            queryDefinition: null,
            requestOptions: new QueryRequestOptions()
            {
                PartitionKey = new PartitionKey(partitionKey),
                MaxItemCount = maxItemCount,
            });

        while (resultSet.HasMoreResults)
        {
            foreach (var item in await resultSet.ReadNextAsync().ConfigureAwait(false))
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<T> ReadAllItemsFromQuery(string query, int maxItemCount = 5)
    {
        using var resultSet = container.GetItemQueryIterator<T>(
                queryDefinition: new QueryDefinition(query),
                requestOptions: new QueryRequestOptions()
                {
                    MaxItemCount = maxItemCount,
                });
        while (resultSet.HasMoreResults)
        {
            foreach (var item in await resultSet.ReadNextAsync().ConfigureAwait(false))
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public async Task<(IQueryable<T> Results, string ContinuationToken)> WhereAsync(Expression<Func<T, bool>> pred, int maxRecords = 0, string partitionKey = "", string continuationToken = "", CancellationToken cancellationToken = default)
    {
        var options = new QueryRequestOptions();

        if (!string.IsNullOrWhiteSpace(partitionKey))
        {
            options.PartitionKey = GeneratePartition(partitionKey);
        }

        if (maxRecords == 0)
        {
            return (container.GetItemLinqQueryable<T>(true, null, options).Where(pred), string.Empty);
        }
        else
        {
            options.MaxItemCount = maxRecords;

            return await BuildResult(container.GetItemLinqQueryable<T>(true, continuationToken?.Trim(), options).Where(pred).ToFeedIterator(), cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<(IQueryable<T> Results, string ContinuationToken)> WhereAsync(string query, int maxRecords = 0, string partitionKey = "", string continuationToken = "", CancellationToken cancellationToken = default)
        => await WhereAsync<T>(query, maxRecords, partitionKey, continuationToken, cancellationToken);

    /// <inheritdoc/>
    public async Task<(IQueryable<TEntity> Results, string ContinuationToken)> WhereAsync<TEntity>(string query, int maxRecords = 0, string partitionKey = "", string continuationToken = "", CancellationToken cancellationToken = default)
    {
        var options = new QueryRequestOptions();
        var queryDefinition = new QueryDefinition(query);

        if (!string.IsNullOrEmpty(partitionKey))
        {
            options.PartitionKey = GeneratePartition(partitionKey);
        }

        if (maxRecords == 0)
        {
            return await BuildResult(container.GetItemQueryIterator<TEntity>(queryDefinition, null, options), cancellationToken);
        }
        else
        {
            options.MaxItemCount = maxRecords;

            return await BuildResult(container.GetItemQueryIterator<TEntity>(query, continuationToken?.Trim(), options), cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task<IQueryable<T>> GetAllAsync(CancellationToken cancellationToken) => await Task.Run(GetAll, cancellationToken);

    /// <inheritdoc/>
    public async Task<IQueryable<T>> GetAllAsync([NotNull] Func<IQueryable<T>, IQueryable<T>> queryFunction, CancellationToken cancellationToken)
    {
        var entities = await Task.Run(() => queryFunction(container.GetItemLinqQueryable<T>(true)).ToList(), cancellationToken);
        return entities.AsQueryable();
    }

    /// <inheritdoc/>
    public async Task<T> GetByIdAsync<TEntityId>(TEntityId id, CancellationToken cancellationToken) => await GetByIdAsync(id.ToString(), null, cancellationToken);

    /// <inheritdoc/>
    public async Task AddAsync(T entity, CancellationToken cancellationToken) => await AddOrUpdateAsync(entity, cancellationToken);

    /// <inheritdoc/>
    public async Task AddBatchAsync(IEnumerable<T> entities, CancellationToken cancellationToken) => await AddOrUpdateBulkAsync(entities, cancellationToken);

    /// <inheritdoc/>
    public async Task DeleteAsync<TEntityId>(TEntityId id, CancellationToken cancellationToken) => await DeleteAsync(id is string idString ? idString : id.ToString(), null, cancellationToken);

    private static async Task<(IQueryable<TEntity> Results, string ContinuationToken)> BuildResult<TEntity>(FeedIterator<TEntity> feedIterator, CancellationToken cancellationToken)
    {
        var continuation = string.Empty;
        var result = new List<TEntity>();

        while (feedIterator.HasMoreResults)
        {
            var response = await feedIterator.ReadNextAsync(cancellationToken);

            result.AddRange(response);

            if (response.Count > 0)
            {
                continuation = response.ContinuationToken;
                return (result.AsQueryable(), continuation);
            }
        }

        return (result.AsQueryable(), continuation);
    }

    private static PartitionKey GeneratePartition(string partitionKey)
    {
        return string.IsNullOrWhiteSpace(partitionKey) ? default : new PartitionKey(partitionKey);
    }
}
