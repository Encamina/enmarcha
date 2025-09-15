using System.Globalization;

using Azure;
using Azure.Core;
using Azure.Data.Tables;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Agents.Abstractions.Cards;
using Encamina.Enmarcha.Agents.Abstractions.Greetings;

using Microsoft.Agents.Core.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Encamina.Enmarcha.Agents.Greetings;

/// <summary>
/// Localized options for greetings messages based on <see cref="HeroCard">hero cards</see>.
/// </summary>
internal class LocalizedHeroCardGreetingsOptionsFromTableStorage : ILocalizedHeroCardGreetingsOptions
{
    private const string CacheKey = @"CacheKey_Greetings";

    private readonly double cacheAbsoluteExpirationSeconds;
    private readonly string? tableConnectionString;
    private readonly Uri? tableEndpoint;
    private readonly string tableName;
    private readonly TokenCredential? tokenCredential;

    private readonly IMemoryCache? memoryCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedHeroCardGreetingsOptionsFromTableStorage"/> class.
    /// </summary>
    /// <param name="tableConnectionString">The Table Storage connection string.</param>
    /// <param name="tableName">The name of the table in the Table storage that contains the localized parameters for the greetings message.</param>
    /// <param name="defaultLocale">The default locale.</param>
    /// <param name="cacheAbsoluteExpirationSeconds">
    /// The absolute expiration time, relative to now in seconds for a cache to store values retrieved from the Table Storage, to improve performance. Default <c>86400</c> (i.e., 24 hours - 1 day).
    /// </param>
    /// <param name="memoryCache">An optional valid instance of a memory cache to improve performance by storing parameters and values retrieved from the Table Storage.</param>
    public LocalizedHeroCardGreetingsOptionsFromTableStorage(string tableConnectionString, string tableName, string defaultLocale, double cacheAbsoluteExpirationSeconds = 86400, IMemoryCache? memoryCache = null)
        : this(tableConnectionString, tableName, CultureInfo.GetCultureInfo(defaultLocale), cacheAbsoluteExpirationSeconds, memoryCache)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedHeroCardGreetingsOptionsFromTableStorage"/> class.
    /// </summary>
    /// <param name="tableEndpoint">The URI of the Azure Table Storage endpoint.</param>
    /// <param name="credential">The <see cref="TokenCredential"/> used to authenticate requests to the Table Storage.</param>
    /// <param name="tableName">The name of the table in the Table storage that contains the localized parameters for the greetings message.</param>
    /// <param name="defaultLocale">The default locale.</param>
    /// <param name="cacheAbsoluteExpirationSeconds">
    /// The absolute expiration time, relative to now in seconds for a cache to store values retrieved from the Table Storage, to improve performance. Default <c>86400</c> (i.e., 24 hours - 1 day).
    /// </param>
    /// <param name="memoryCache">An optional valid instance of a memory cache to improve performance by storing parameters and values retrieved from the Table Storage.</param>
    public LocalizedHeroCardGreetingsOptionsFromTableStorage(Uri tableEndpoint, TokenCredential credential, string tableName, string defaultLocale, double cacheAbsoluteExpirationSeconds = 86400, IMemoryCache? memoryCache = null)
        : this(tableEndpoint, credential, tableName, CultureInfo.GetCultureInfo(defaultLocale), cacheAbsoluteExpirationSeconds, memoryCache)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedHeroCardGreetingsOptionsFromTableStorage"/> class.
    /// </summary>
    /// <param name="tableConnectionString">The Table Storage connection string.</param>
    /// <param name="tableName">The name of the table in the Table storage that contains the localized parameters for the greetings message.</param>
    /// <param name="defaultLocale">The default locale.</param>
    /// <param name="cacheAbsoluteExpirationSeconds">
    /// The absolute expiration time, relative to now in seconds for a cache to store values retrieved from the Table Storage, to improve performance. Default <c>86400</c> (i.e., 24 hours - 1 day).
    /// </param>
    /// <param name="memoryCache">An optional valid instance of a memory cache to improve performance by storing parameters and values retrieved from the Table Storage.</param>
    public LocalizedHeroCardGreetingsOptionsFromTableStorage(string tableConnectionString, string tableName, CultureInfo defaultLocale, double cacheAbsoluteExpirationSeconds = 86400, IMemoryCache? memoryCache = null)
        : this(tableName, defaultLocale, cacheAbsoluteExpirationSeconds, memoryCache)
    {
        Guard.IsNotNullOrWhiteSpace(tableConnectionString);

        this.tableConnectionString = tableConnectionString;
        LocalizedOptions = BuildOptions();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedHeroCardGreetingsOptionsFromTableStorage"/> class.
    /// </summary>
    /// <param name="tableEndpoint">The URI of the Azure Table Storage endpoint.</param>
    /// <param name="credential">The <see cref="TokenCredential"/> used to authenticate requests to the Table Storage.</param>
    /// <param name="tableName">The name of the table in the Table storage that contains the localized parameters for the greetings message.</param>
    /// <param name="defaultLocale">The default locale.</param>
    /// <param name="cacheAbsoluteExpirationSeconds">
    /// The absolute expiration time, relative to now in seconds for a cache to store values retrieved from the Table Storage, to improve performance. Default <c>86400</c> (i.e., 24 hours - 1 day).
    /// </param>
    /// <param name="memoryCache">An optional valid instance of a memory cache to improve performance by storing parameters and values retrieved from the Table Storage.</param>
    public LocalizedHeroCardGreetingsOptionsFromTableStorage(Uri tableEndpoint, TokenCredential credential, string tableName, CultureInfo defaultLocale, double cacheAbsoluteExpirationSeconds = 86400, IMemoryCache? memoryCache = null)
        : this(tableName, defaultLocale, cacheAbsoluteExpirationSeconds, memoryCache)
    {
        Guard.IsNotNull(tableEndpoint);
        Guard.IsNotNull(credential);

        this.tableEndpoint = tableEndpoint;
        this.tokenCredential = credential;

        LocalizedOptions = BuildOptions();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedHeroCardGreetingsOptionsFromTableStorage"/> class.
    /// </summary>
    /// <param name="tableName">The name of the table in the Table storage that contains the localized parameters for the greetings message.</param>
    /// <param name="defaultLocale">The default locale.</param>
    /// <param name="cacheAbsoluteExpirationSeconds">
    /// The absolute expiration time, relative to now in seconds for a cache to store values retrieved from the Table Storage, to improve performance. Default <c>86400</c> (i.e., 24 hours - 1 day).
    /// </param>
    /// <param name="memoryCache">An optional valid instance of a memory cache to improve performance by storing parameters and values retrieved from the Table Storage.</param>
    private LocalizedHeroCardGreetingsOptionsFromTableStorage(string tableName, CultureInfo defaultLocale, double cacheAbsoluteExpirationSeconds, IMemoryCache? memoryCache)
    {
        Guard.IsNotNullOrWhiteSpace(tableName);
        Guard.IsNotNull(defaultLocale);
        Guard.IsNotNull(memoryCache);

        this.cacheAbsoluteExpirationSeconds = cacheAbsoluteExpirationSeconds;
        this.tableName = tableName;

        this.memoryCache = memoryCache;

        DefaultLocale = defaultLocale;
        LocalizedOptions = new Dictionary<CultureInfo, IEnumerable<IHeroCardOptions>>();
    }

    /// <inheritdoc/>
    public CultureInfo DefaultLocale { get; }

    /// <inheritdoc/>
    public IDictionary<CultureInfo, IEnumerable<IHeroCardOptions>> LocalizedOptions { get; }

    private IDictionary<CultureInfo, IEnumerable<IHeroCardOptions>> BuildOptions()
    {
        return memoryCache?.GetOrCreate(CacheKey, cacheEntry =>
        {
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheAbsoluteExpirationSeconds);
            return Init();
        }) ?? Init();
    }

    private IDictionary<CultureInfo, IEnumerable<IHeroCardOptions>> Init()
    {
        var tableClient = !string.IsNullOrWhiteSpace(tableConnectionString)
            ? new TableClient(tableConnectionString, tableName)
            : new TableClient(tableEndpoint!, tableName, tokenCredential!);

        tableClient.CreateIfNotExists();

        var entities = tableClient.Query<HeroCardGreetingsOptionsTableEntity>().ToLookup(entity => (entity.PartitionKey, entity.Order));

        var dictionary = new Dictionary<string, IList<IHeroCardOptions>>();

        foreach (var entity in entities)
        {
            var option = new InternalHeroCardGreetingsOptions();

            foreach (var item in entity)
            {
                switch (item.Component)
                {
                    case ComponentKind.Title:
                        option.Title = item.Value;
                        break;
                    case ComponentKind.Subtitle:
                        option.Subtitle = item.Value;
                        break;
                    case ComponentKind.Text:
                        option.Text = item.Value;
                        break;
                    case ComponentKind.ImageUrl:
                        option.Images.Insert(item.ImageOrder, new CardImage(item.Value));
                        break;
                }
            }

            if (dictionary.ContainsKey(entity.Key.PartitionKey))
            {
                dictionary[entity.Key.PartitionKey].Add(option);
            }
            else
            {
                dictionary[entity.Key.PartitionKey] = new List<IHeroCardOptions>() { option };
            }
        }

        return dictionary.ToDictionary(k => CultureInfo.GetCultureInfo(k.Key), e => e.Value.AsEnumerable());
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "This is a private enum type that should not be exposed externally, and rarely modified.")]
    private enum ComponentKind
    {
        Unknown,
        Title,
        Subtitle,
        Text,
        ImageUrl,
    }

    private sealed class InternalHeroCardGreetingsOptions : IHeroCardOptions
    {
        private readonly List<CardImage> cardImages = [];

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string Text { get; set; }

        public IList<CardImage> Images => cardImages;
    }

    private sealed class HeroCardGreetingsOptionsTableEntity : ITableEntity
    {
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public ComponentKind Component { get; set; } = ComponentKind.Unknown;

        public string Value { get; set; } = string.Empty;

        public int Order { get; set; } = 0;

        public int ImageOrder { get; set; } = 0;
    }
}
