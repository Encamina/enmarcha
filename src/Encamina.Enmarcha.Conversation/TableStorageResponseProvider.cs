﻿using System.Globalization;

using Azure.Core;
using Azure.Data.Tables;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Conversation.Abstractions;

using Microsoft.Extensions.Caching.Memory;

namespace Encamina.Enmarcha.Conversation;

/// <summary>
/// Custom intent responder provider based on values configured and stored in an Azure Table Storage.
/// </summary>
internal class TableStorageResponseProvider : IIntentResponsesProvider
{
    private const string CacheKey = @"CacheKey_LocalizedIntentResponses";

    private readonly double cacheAbsoluteExpirationSeconds;
    private readonly string defaultLocale;
    private readonly string intentOrderSeparator;
    private readonly string? tableConnectionString;
    private readonly Uri? tableEndpoint;
    private readonly string tableName;
    private readonly TokenCredential? tokenCredential;
    private readonly IMemoryCache memoryCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableStorageResponseProvider"/> class.
    /// </summary>
    /// <param name="tableConnectionString">The connection string to an Azure Table Storage.</param>
    /// <param name="tableName">The name of the table in the azure Table Storage.</param>
    /// <param name="defaultLocale">Default locale.</param>
    /// <param name="intentCounterSeparator">
    /// An optional value that represents the intent counter separation (i.e., a value that helps to separate the intent
    /// label from a numeric value that represents its order or instance number). Defaults to '<c>-</c>'.
    /// </param>
    /// <param name="cacheAbsoluteExpirationSeconds">
    /// An optional value for absolute expiration time in seconds for the cache. Defaults to '<c>86400</c>' seconds.
    /// </param>
    /// <param name="memoryCache">An optional valid instance of a <see cref="IMemoryCache"/>.</param>
    public TableStorageResponseProvider(
        string tableConnectionString,
        string tableName,
        string defaultLocale,
        string intentCounterSeparator = @"-",
        double cacheAbsoluteExpirationSeconds = 86400,
        IMemoryCache? memoryCache = null)
        : this(tableName, defaultLocale, intentCounterSeparator, cacheAbsoluteExpirationSeconds, memoryCache)
    {
        Guard.IsNotNullOrWhiteSpace(tableConnectionString);

        this.tableConnectionString = tableConnectionString;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableStorageResponseProvider"/> class.
    /// </summary>
    /// <param name="tableEndpoint">The URI of the Azure Table Storage endpoint.</param>
    /// <param name="credential">The <see cref="TokenCredential"/> used to authenticate requests to the Table Storage.</param>
    /// <param name="tableName">The name of the table in the azure Table Storage.</param>
    /// <param name="defaultLocale">Default locale.</param>
    /// <param name="intentCounterSeparator">
    /// An optional value that represents the intent counter separation (i.e., a value that helps to separate the intent
    /// label from a numeric value that represents its order or instance number). Defaults to '<c>-</c>'.
    /// </param>
    /// <param name="cacheAbsoluteExpirationSeconds">
    /// An optional value for absolute expiration time in seconds for the cache. Defaults to '<c>86400</c>' seconds.
    /// </param>
    /// <param name="memoryCache">An optional valid instance of a <see cref="IMemoryCache"/>.</param>
    public TableStorageResponseProvider(
        Uri tableEndpoint,
        TokenCredential credential,
        string tableName,
        string defaultLocale,
        string intentCounterSeparator = "-",
        double cacheAbsoluteExpirationSeconds = 86400,
        IMemoryCache? memoryCache = null)
        : this(tableName, defaultLocale, intentCounterSeparator, cacheAbsoluteExpirationSeconds, memoryCache)
    {
        Guard.IsNotNull(tableEndpoint);
        Guard.IsNotNull(credential);

        this.tokenCredential = credential;
        this.tableEndpoint = tableEndpoint;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableStorageResponseProvider"/> class.
    /// </summary>
    /// <param name="tableName">The name of the table in the azure Table Storage.</param>
    /// <param name="defaultLocale">Default locale.</param>
    /// <param name="intentCounterSeparator">
    /// An optional value that represents the intent counter separation (i.e., a value that helps to separate the intent
    /// label from a numeric value that represents its order or instance number). Defaults to '<c>-</c>'.
    /// </param>
    /// <param name="cacheAbsoluteExpirationSeconds">
    /// An optional value for absolute expiration time in seconds for the cache. Defaults to '<c>86400</c>' seconds.
    /// </param>
    /// <param name="memoryCache">An optional valid instance of a <see cref="IMemoryCache"/>.</param>
    private TableStorageResponseProvider(
        string tableName,
        string defaultLocale,
        string intentCounterSeparator,
        double cacheAbsoluteExpirationSeconds,
        IMemoryCache? memoryCache)
    {
        Guard.IsNotNullOrWhiteSpace(tableName);
        Guard.IsNotNullOrWhiteSpace(defaultLocale);
        Guard.IsNotNullOrWhiteSpace(intentCounterSeparator);
        Guard.IsNotNull(memoryCache);

        this.cacheAbsoluteExpirationSeconds = cacheAbsoluteExpirationSeconds;
        this.defaultLocale = defaultLocale;
        this.intentOrderSeparator = intentCounterSeparator;
        this.memoryCache = memoryCache;
        this.tableName = tableName;
    }

    /// <inheritdoc/>
    public virtual Task<IReadOnlyCollection<Response>> GetResponsesAsync(string intent, string locale, CancellationToken cancellationToken)
        => GetResponsesAsync(intent, CultureInfo.GetCultureInfo(locale), cancellationToken);

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyCollection<Response>> GetResponsesAsync(string intent, CultureInfo culture, CancellationToken cancellationToken)
    {
        var intentsByLocale = await memoryCache.GetOrCreateAsync(CacheKey, async cacheEntry =>
        {
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheAbsoluteExpirationSeconds);
            return await InitAsync(cancellationToken);
        });

        return intentsByLocale != null && (intentsByLocale.TryGetValue(culture.Name.ToUpperInvariant(), out var responsesByIntent) ||
                                           intentsByLocale.TryGetValue(culture.Parent.Name.ToUpperInvariant(), out responsesByIntent) ||
                                           intentsByLocale.TryGetValue(defaultLocale.ToUpperInvariant(), out responsesByIntent)) && responsesByIntent.TryGetValue(intent.ToUpperInvariant(), out var responses)
            ? responses.OrderBy(r => r.Order).ToList().AsReadOnly()
            : Array.Empty<Response>();
    }

    private async Task<IDictionary<string, IDictionary<string, IList<Response>>>> InitAsync(CancellationToken cancellationToken)
    {
        var tableClient = !string.IsNullOrWhiteSpace(tableConnectionString)
            ? new TableClient(tableConnectionString, tableName)
            : new TableClient(tableEndpoint!, tableName, tokenCredential!);

        await tableClient.CreateIfNotExistsAsync(cancellationToken);

        var entities = await tableClient.QueryAsync<ResponsesTableEntity>(cancellationToken: cancellationToken)
                                        .ToLookupAsync(e => e.Locale.ToUpperInvariant(), cancellationToken);

        var intentsByLocale = new Dictionary<string, IDictionary<string, IList<Response>>>();

        foreach (var entity in entities)
        {
            GetsIntentsByLocaleFromEntity(entity, intentsByLocale);
        }

        return intentsByLocale;
    }

    private void GetsIntentsByLocaleFromEntity(IGrouping<string, ResponsesTableEntity> entity, IDictionary<string, IDictionary<string, IList<Response>>> intentsByLocale)
    {
        foreach (var value in entity)
        {
            var intent = value.Intent;
            var index = intent.IndexOf(intentOrderSeparator, StringComparison.OrdinalIgnoreCase);
            var responseIntent = (index > 0 ? intent[..index] : intent).ToUpperInvariant();

            var response = new Response()
            {
                Order = int.TryParse(intent[(index + 1)..], out var val) ? val : 0,
                Text = value.Response,
            };

            if (intentsByLocale.TryGetValue(entity.Key, out var responsesByIntent))
            {
                if (responsesByIntent.ContainsKey(responseIntent))
                {
                    responsesByIntent[responseIntent].Add(response);
                }
                else
                {
                    responsesByIntent[responseIntent] = [response];
                }
            }
            else
            {
                intentsByLocale[entity.Key] = new Dictionary<string, IList<Response>>() { { responseIntent, new List<Response>() { response } } };
            }
        }
    }

    private sealed class ResponsesTableEntity : ITableEntity
    {
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public Azure.ETag ETag { get; set; }

        public string Response { get; set; } = string.Empty;

        public string Locale => PartitionKey;

        public string Intent => RowKey;
    }
}